namespace StreamierGraphQLServer.GraphQL;

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Resend;
using StreamierGraphQLServer.Exceptions;
using StreamierGraphQLServer.Models;
using StreamierGraphQLServer.Models.Users;
using Zxcvbn;

/// <summary>
/// Represents the root GraphQL mutation type containing all available mutation operations.
/// </summary>
public class Mutation
{
    private const int MinimumPasswordScore = 3;

    private const int MaxSessionsPerUser = 20;

    private const int TempCodeExpirationHours = 2;
    private const int TempCodeLength = 16;

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(UserAlreadyExistsException))]
    [Error(typeof(WeakPasswordException))]
    public async Task<User> SignUp([Service] Contexts.AppDbContext dbContext, SignUpInput input)
    {
        ValidateInput(input);

        if (await dbContext.Users.AnyAsync(u => u.Email == input.Email))
        {
            throw new UserAlreadyExistsException(input.Email);
        }

        var result = Core.EvaluatePassword(input.Password);

        if (result.Score < MinimumPasswordScore)
        {
            throw new WeakPasswordException(result.Feedback);
        }

        var userId = await User.GenerateIdAsync(dbContext);

        var newUser = new User
        {
            Id = userId,
            Email = input.Email,
            IsEmailVerified = false,
            HashedPassword = User.HashPassword(input.Password),
            Username = input.Username,
            PrivacySettings = new UserPrivacySettings() { Id = userId },
            Preferences = new UserPreferences() { Id = userId },
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        return newUser;
    }

    /// <summary>
    /// Creates a new authentication session for an existing user.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(InvalidPasswordException))]
    [Error(typeof(InvalidSessionExpirationException))]
    [Error(typeof(MaxSessionsExceededException))]
    public async Task<Session> SignIn([Service] Contexts.AppDbContext dbContext, SignInInput input)
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u => u.Email == input.Email)
            ?? throw new UserNotFoundException(input.Email);

        if (!User.ValidatePassword(input.Password, user.HashedPassword))
        {
            throw new InvalidPasswordException();
        }

        var now = DateTime.UtcNow;
        var minExpiration = now.AddHours(1);
        var maxExpiration = now.AddDays(365);

        if (input.ExpirationDate < minExpiration || input.ExpirationDate > maxExpiration)
        {
            throw new InvalidSessionExpirationException(minExpiration, maxExpiration);
        }

        var userSessionsCount = await dbContext.Sessions.CountAsync(s => s.UserId == user.Id);
        if (userSessionsCount >= MaxSessionsPerUser)
        {
            throw new MaxSessionsExceededException(MaxSessionsPerUser);
        }

        var session = new Session
        {
            Id = Session.GenerateSessionId(),
            UserId = user.Id,
            ExpiresAt = input.ExpirationDate,
        };

        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Deletes an existing session by its ID.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(InvalidSessionException))]
    public async Task<bool> DeleteSession(
        [Service] Contexts.AppDbContext dbContext,
        DeleteSessionInput input
    )
    {
        var session =
            await dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == input.SessionId)
            ?? throw new InvalidSessionException(input.SessionId);

        dbContext.Sessions.Remove(session);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Creates a temporary code for a specific purpose and user ID.
    /// </summary>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(TempCodeAlreadyExistsException))]
    [Error(typeof(InvalidUserIdException))]
    [Error(typeof(InvalidTempCodePurposeException))]
    public async Task<bool> CreateTempCodeForId(
        [Service] Contexts.AppDbContext dbContext,
        [Service] IResend resend,
        CreateTempCodeForIdInput input
    )
    {
        if (
            await dbContext.TempCodes.AnyAsync(c =>
                c.Purpose == input.Purpose && c.ForId == input.ForId
            )
        )
        {
            throw new TempCodeAlreadyExistsException(input.Purpose.ToString(), input.ForId);
        }

        var code = GenerateTempCode();
        var codeSalt = TempCode.GenerateCodeSalt();
        var hashedCode = TempCode.HashCode(code, codeSalt);

        var message = await CreateEmailMessage(dbContext, input);

        dbContext.TempCodes.Add(
            new TempCode
            {
                Id = TempCode.GenerateCode(),
                Purpose = input.Purpose,
                ForId = input.ForId,
                HashedCode = hashedCode,
                CodeSalt = codeSalt,
                ExpiresAt = DateTime.UtcNow.AddHours(TempCodeExpirationHours),
            }
        );

        await dbContext.SaveChangesAsync();
        await resend.EmailSendAsync(message);

        return true;
    }

    private static string GenerateTempCode() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(TempCodeLength))[..TempCodeLength];

    private static async Task<EmailMessage> CreateEmailMessage(
        Contexts.AppDbContext dbContext,
        CreateTempCodeForIdInput input
    )
    {
        var user =
            await dbContext
                .Users.Where(u => u.Id == input.ForId)
                .Select(u => new { u.Email })
                .FirstOrDefaultAsync() ?? throw new InvalidUserIdException(input.ForId);

        var subject = input.Purpose switch
        {
            TempCode.TempCodePurpose.ChangePassword => "Change Password",
            TempCode.TempCodePurpose.ChangeEmail => "Change Email",
            TempCode.TempCodePurpose.EmailVerification => "Email Verification",
            TempCode.TempCodePurpose.DeleteAccount => "Removing Account",
            _ => throw new InvalidTempCodePurposeException(input.Purpose.ToString()),
        };

        return new EmailMessage
        {
            From = "core@botstudioo.com",
            To = { user.Email },
            Subject = subject,
            HtmlBody = "<strong>it works!</strong>",
        };
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
