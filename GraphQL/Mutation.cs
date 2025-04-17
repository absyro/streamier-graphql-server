namespace StreamierGraphQLServer.GraphQL;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using RandomString4Net;
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
    public async Task<User> SignUp([Service] Contexts.AppDbContext dbContext, SignUpInput input)
    {
        ValidateInput(input);

        if (await dbContext.Users.AnyAsync(u => u.Email == input.Email))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Email already exists")
                    .SetCode("EMAIL_ALREADY_EXISTS")
                    .SetExtension("email", input.Email)
                    .Build()
            );
        }

        if (await dbContext.Users.AnyAsync(u => u.Username == input.Username))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Username already taken")
                    .SetCode("USERNAME_TAKEN")
                    .SetExtension("username", input.Username)
                    .Build()
            );
        }

        var result = Core.EvaluatePassword(input.Password);

        if (result.Score < 3)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Password is too weak")
                    .SetCode("WEAK_PASSWORD")
                    .SetExtension("feedback", result.Feedback)
                    .Build()
            );
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
    public async Task<UserSession> SignIn(
        [Service] Contexts.AppDbContext dbContext,
        SignInInput input
    )
    {
        ValidateInput(input);

        var now = DateTime.UtcNow;

        var minExpiration = now.AddHours(1);
        var maxExpiration = now.AddDays(365);

        if (input.ExpirationDate < minExpiration || input.ExpirationDate > maxExpiration)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Invalid expiration date")
                    .SetCode("INVALID_EXPIRATION_DATE")
                    .Build()
            );
        }

        var user =
            await dbContext
                .Users.Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.Email == input.Email)
            ?? throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );

        if (!BCrypt.Net.BCrypt.Verify(input.Password, user.HashedPassword))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Invalid password")
                    .SetCode("INVALID_PASSWORD")
                    .Build()
            );
        }

        if (user.TwoFactorAuthentication != null)
        {
            if (input.TwoFactorAuthenticationCode == null)
            {
                throw new GraphQLException(
                    ErrorBuilder
                        .New()
                        .SetMessage("Two-factor authentication code required")
                        .SetCode("2FA_REQUIRED")
                        .Build()
                );
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
                    throw new GraphQLException(
                        ErrorBuilder
                            .New()
                            .SetMessage("Invalid two-factor authentication code")
                            .SetCode("INVALID_2FA_CODE")
                            .Build()
                    );
                }

                user.TwoFactorAuthentication.RecoveryCodes.Remove(
                    input.TwoFactorAuthenticationCode
                );

                await dbContext.SaveChangesAsync();
            }
        }

        const int MaxSessionsPerUser = 5;

        if (user.Sessions.Count >= MaxSessionsPerUser)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage(
                        $"Maximum number of sessions per user ({MaxSessionsPerUser}) reached"
                    )
                    .SetCode("MAX_SESSIONS_PER_USER")
                    .Build()
            );
        }

        string id;

        do
        {
            id = RandomString.GetString(Types.ALPHANUMERIC_MIXEDCASE_WITH_SYMBOLS, 128);
        } while (await dbContext.Users.AnyAsync(u => u.Sessions.Any(s => s.Id == id)));

        var session = new UserSession { Id = id, ExpiresAt = input.ExpirationDate };

        user.Sessions.Add(session);

        await dbContext.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Enables two-factor authentication for a user account.
    /// </summary>
    public async Task<EnableTwoFactorAuthenticationResult> EnableTwoFactorAuthentication(
        [Service] Contexts.AppDbContext dbContext,
        EnableTwoFactorAuthenticationInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u =>
                u.Sessions.Any(s => s.Id == input.SessionId)
            )
            ?? throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );

        if (user.TwoFactorAuthentication != null)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Two-factor authentication is already enabled.")
                    .SetCode("TWO_FACTOR_ALREADY_ENABLED")
                    .Build()
            );
        }

        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(secretKey);

        var recoveryCodes = UserTwoFactorAuthentication.GenerateRecoveryCodes();

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
    public async Task<bool> DisableTwoFactorAuthentication(
        [Service] Contexts.AppDbContext dbContext,
        DisableTwoFactorAuthenticationInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u =>
                u.Sessions.Any(s => s.Id == input.SessionId)
            )
            ?? throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );

        if (!BCrypt.Net.BCrypt.Verify(input.Password, user.HashedPassword))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Invalid password")
                    .SetCode("INVALID_PASSWORD")
                    .Build()
            );
        }

        if (user.TwoFactorAuthentication == null)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Two-factor authentication is not enabled.")
                    .SetCode("TWO_FACTOR_NOT_ENABLED")
                    .Build()
            );
        }

        user.TwoFactorAuthentication = null;

        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Generates new recovery codes for two-factor authentication.
    /// </summary>
    public async Task<List<string>> GenerateNewRecoveryCodes(
        [Service] Contexts.AppDbContext dbContext,
        GenerateNewRecoveryCodesInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u =>
                u.Sessions.Any(s => s.Id == input.SessionId)
            )
            ?? throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );

        if (user.TwoFactorAuthentication == null)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Two-factor authentication is not enabled")
                    .SetCode("2FA_NOT_ENABLED")
                    .Build()
            );
        }

        var recoveryCodes = UserTwoFactorAuthentication.GenerateRecoveryCodes();

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
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Validation failed")
                    .SetCode("VALIDATION_FAILED")
                    .SetExtension("errors", validationResults)
                    .Build()
            );
        }
    }
}
