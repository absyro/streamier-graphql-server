namespace StreamierGraphQLServer.GraphQL;

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Resend;
using StreamierGraphQLServer.Models;
using StreamierGraphQLServer.Models.Users;

/// <summary>
/// Represents the root GraphQL mutation type containing all available mutation operations.
/// </summary>
public class Mutation
{
    private const int MaxSessionsPerUser = 20;
    private const int TempCodeExpirationHours = 2;
    private const int TempCodeLength = 16;

    /// <summary>
    /// Custom exception type for mutation-related errors.
    /// Provides consistent error handling for GraphQL mutations.
    /// </summary>
    /// <param name="message">The error message describing what went wrong.</param>
    public class MutationException(string message) : Exception(message);

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="dbContext">The database context for user operations.</param>
    /// <param name="input">The user registration details.</param>
    /// <returns>The newly created user.</returns>
    [Error(typeof(MutationException))]
    public async Task<User> SignUp([Service] Contexts.AppDbContext dbContext, SignUpInput input)
    {
        ValidateInput(input);

        if (await dbContext.Users.AnyAsync(u => u.Email == input.Email))
        {
            throw new MutationException("A user with the provided email address already exists.");
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
    /// <param name="dbContext">The database context for user operations.</param>
    /// <param name="input">The authentication details.</param>
    /// <returns>The newly created session.</returns>
    [Error(typeof(MutationException))]
    public async Task<Session> SignIn([Service] Contexts.AppDbContext dbContext, SignInInput input)
    {
        ValidateInput(input);

        var user =
            await dbContext.Users.FirstOrDefaultAsync(u => u.Email == input.Email)
            ?? throw new MutationException("A user with the provided email address was not found.");

        if (!User.ValidatePassword(input.Password, user.HashedPassword))
        {
            throw new MutationException("The provided password is incorrect.");
        }

        var userSessionsCount = await dbContext.Sessions.CountAsync(s => s.UserId == user.Id);
        if (userSessionsCount >= MaxSessionsPerUser)
        {
            throw new MutationException(
                $"The maximum number of sessions ({MaxSessionsPerUser}) for this user has been reached."
            );
        }

        var session = new Session
        {
            Id = Session.GenerateSessionId(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(input.ExpirationDays),
        };

        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Deletes an existing session by its ID.
    /// </summary>
    /// <param name="dbContext">The database context for session operations.</param>
    /// <param name="input">The input containing the session ID to delete.</param>
    /// <returns>true if deletion was successful.</returns>
    [Error(typeof(MutationException))]
    public async Task<bool> DeleteSession(
        [Service] Contexts.AppDbContext dbContext,
        DeleteSessionInput input
    )
    {
        var session =
            await dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == input.SessionId)
            ?? throw new MutationException("Invalid session ID.");

        dbContext.Sessions.Remove(session);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Creates a temporary code for a specific purpose and user ID.
    /// </summary>
    [Error(typeof(MutationException))]
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
            throw new MutationException(
                "Same code with the same purpose has already been registered for the same ID."
            );
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
                .FirstOrDefaultAsync() ?? throw new MutationException("Invalid user ID.");

        var subject = input.Purpose switch
        {
            TempCode.TempCodePurpose.ChangePassword => "Change Password",
            TempCode.TempCodePurpose.ChangeEmail => "Change Email",
            TempCode.TempCodePurpose.EmailVerification => "Email Verification",
            TempCode.TempCodePurpose.DeleteAccount => "Removing Account",
            _ => throw new MutationException("Invalid purpose."),
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
            throw new MutationException(
                string.Join(" ", validationResults.Select(r => r.ErrorMessage))
            );
        }
    }
}
