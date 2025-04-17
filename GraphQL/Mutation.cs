namespace StreamierGraphQLServer.GraphQL;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using RandomString4Net;
using StreamierGraphQLServer.Exceptions;
using StreamierGraphQLServer.Inputs;
using StreamierGraphQLServer.Models.Users;
using Zxcvbn;

/// <summary>
/// Represents the root GraphQL mutation type containing all available mutation operations.
/// This class handles all write operations including user registration, authentication,
/// session management, and user profile updates.
/// </summary>
public class Mutation
{
    /// <summary>
    /// Creates a new user account with the provided credentials and information.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(EmailAlreadyExistsException))]
    [Error(typeof(UsernameTakenException))]
    [Error(typeof(WeakPasswordException))]
    public async Task<User> SignUp([Service] Contexts.AppDbContext dbContext, SignUpInput input)
    {
        ValidateInput(input);

        if (await dbContext.Users.AnyAsync(u => u.Email == input.Email))
        {
            throw new EmailAlreadyExistsException(input.Email);
        }

        if (await dbContext.Users.AnyAsync(u => u.Username == input.Username))
        {
            throw new UsernameTakenException(input.Username);
        }

        var result = Core.EvaluatePassword(input.Password);

        if (result.Score < 3)
        {
            throw new WeakPasswordException(result.Feedback);
        }

        string id;

        do
        {
            id = RandomString.GetString(Types.ALPHANUMERIC_LOWERCASE, 8);
        } while (await dbContext.Users.AnyAsync(u => u.Id == id));

        var user = new User
        {
            Id = id,
            Email = input.Email,
            Username = input.Username,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(
                input.Password,
                BCrypt.Net.BCrypt.GenerateSalt(12)
            ),
            PrivacySettings = new UserPrivacySettings() { Id = id },
            Preferences = new UserPreferences() { Id = id },
        };

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Creates a new authentication session for an existing user.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(InvalidPasswordException))]
    [Error(typeof(InvalidSessionExpirationException))]
    [Error(typeof(MaxSessionsExceededException))]
    public async Task<UserSession> SignIn(
        [Service] Contexts.AppDbContext dbContext,
        SignInInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext
                .Users.Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.Email == input.Email)
            ?? throw new UserNotFoundException();

        if (!BCrypt.Net.BCrypt.Verify(input.Password, user.HashedPassword))
        {
            throw new InvalidPasswordException();
        }

        if (user.TwoFactorAuthentication != null)
        {
            if (input.TwoFactorAuthenticationCode == null)
            {
                throw new InvalidTwoFactorAuthenticationCodeException();
            }

            var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorAuthentication.Secret));

            if (
                !totp.VerifyTotp(
                    input.TwoFactorAuthenticationCode,
                    out _,
                    new VerificationWindow(1, 1)
                )
            )
            {
                if (
                    !user.TwoFactorAuthentication.RecoveryCodes.Contains(
                        input.TwoFactorAuthenticationCode
                    )
                )
                {
                    throw new InvalidTwoFactorAuthenticationCodeException();
                }

                user.TwoFactorAuthentication.RecoveryCodes.Remove(
                    input.TwoFactorAuthenticationCode
                );

                await dbContext.SaveChangesAsync();
            }
        }

        return await User.GenerateSessionAsync(dbContext, user, input.ExpirationDate);
    }

    /// <summary>
    /// Enables two-factor authentication for a user account.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(InvalidSessionException))]
    public async Task<EnableTwoFactorAuthenticationResult> EnableTwoFactorAuthentication(
        [Service] Contexts.AppDbContext dbContext,
        EnableTwoFactorAuthenticationInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u =>
                u.Sessions.Any(s => s.Id == input.SessionId)
            ) ?? throw new UserNotFoundException();

        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(secretKey);

        var recoveryCodes = User.GenerateRecoveryCodes();

        user.TwoFactorAuthentication = new UserTwoFactorAuthentication
        {
            Id = user.Id,
            RecoveryCodes = recoveryCodes,
            Secret = base32Secret,
        };

        await dbContext.SaveChangesAsync();

        return new EnableTwoFactorAuthenticationResult
        {
            SecretKey = base32Secret,
            RecoveryCodes = recoveryCodes,
        };
    }

    /// <summary>
    /// Disables two-factor authentication for a user account.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(InvalidSessionException))]
    [Error(typeof(InvalidPasswordException))]
    public async Task<bool> DisableTwoFactorAuthentication(
        [Service] Contexts.AppDbContext dbContext,
        DisableTwoFactorAuthenticationInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u =>
                u.Sessions.Any(s => s.Id == input.SessionId)
            ) ?? throw new UserNotFoundException();

        if (!BCrypt.Net.BCrypt.Verify(input.Password, user.HashedPassword))
        {
            throw new InvalidPasswordException();
        }

        user.TwoFactorAuthentication = null;

        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Generates new recovery codes for two-factor authentication.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(InvalidSessionException))]
    public async Task<List<string>> GenerateNewRecoveryCodes(
        [Service] Contexts.AppDbContext dbContext,
        GenerateRecoveryCodesInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u =>
                u.Sessions.Any(s => s.Id == input.SessionId)
            ) ?? throw new UserNotFoundException();

        if (user.TwoFactorAuthentication == null)
        {
            throw new InvalidOperationException("Two-factor authentication is not enabled");
        }

        var recoveryCodes = User.GenerateRecoveryCodes();

        user.TwoFactorAuthentication.RecoveryCodes = recoveryCodes;

        await dbContext.SaveChangesAsync();

        return recoveryCodes;
    }

    /// <summary>
    /// Represents the result of enabling two-factor authentication.
    /// </summary>
    public class EnableTwoFactorAuthenticationResult
    {
        /// <summary>
        /// The secret key used to generate two-factor codes.
        /// </summary>
        public string SecretKey { get; set; } = null!;

        /// <summary>
        /// The recovery codes for two-factor authentication.
        /// </summary>
        public List<string> RecoveryCodes { get; set; } = null!;
    }

    private static void ValidateInput(object input)
    {
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(input, validationContext, validationResults, true))
        {
            throw new ValidationFailedException(validationResults);
        }
    }
}
