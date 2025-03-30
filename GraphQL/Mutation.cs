namespace StreamierServer.GraphQL;

using System.Security.Cryptography;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resend;
using Snowflake.Net;

/// <summary>
/// Represents the GraphQL mutation operations.
/// </summary>
public class Mutation
{
    private readonly IdWorker _snowflakeGenerator = new(1, 1);

    private const int MaxSessionsPerUser = 20;

    private const int TempCodeExpirationHours = 2;
    private const int TempCodeLength = 16;

    /// <summary>
    /// Represents a mutation exception.
    /// </summary>
    /// <param name="message"></param>
    public class MutationException(string message) : Exception(message);

    /// <summary>
    /// Represents the input for the CreateSession mutation.
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="Password"></param>
    /// <param name="Mode"></param>
    /// <param name="ExpirationDays"></param>
    public record CreateSessionInput(
        string Email,
        string Password,
        CreateSessionMode Mode = CreateSessionMode.SignIn,
        int ExpirationDays = 30
    );

    /// <summary>
    /// Represents the mode for the CreateSession mutation.
    /// </summary>
    public enum CreateSessionMode
    {
        /// <summary>
        /// Signs up a new user.
        /// </summary>
        SignUp,

        /// <summary>
        /// Signs in an existing user.
        /// </summary>
        SignIn,
    }

    /// <summary>
    /// Creates a new session.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="validator"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="MutationException"></exception>
    [Error(typeof(MutationException))]
    public async Task<Models.Session> CreateSession(
        [Service] Contexts.AppDbContext dbContext,
        [Service] IValidator<CreateSessionInput> validator,
        CreateSessionInput input
    )
    {
        await validator.ValidateAndThrowAsync(input);

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == input.Email);

        user = input.Mode switch
        {
            CreateSessionMode.SignUp => await HandleSignUp(dbContext, input, user),
            CreateSessionMode.SignIn => await HandleSignIn(dbContext, input, user),
            _ => throw new MutationException("Invalid session mode."),
        };

        if (user == null)
        {
            throw new MutationException("Failed to find or create user.");
        }

        var session = new Models.Session
        {
            Id = _snowflakeGenerator.NextId().ToString(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(input.ExpirationDays),
        };

        dbContext.Sessions.Add(session);

        await dbContext.SaveChangesAsync();

        return session;
    }

    private async Task<Models.User> HandleSignUp(
        Contexts.AppDbContext dbContext,
        CreateSessionInput input,
        Models.User? existingUser
    )
    {
        if (existingUser != null)
        {
            throw new MutationException("A user with the provided email address already exists.");
        }

        var newUser = new Models.User
        {
            Id = _snowflakeGenerator.NextId().ToString(),
            Email = input.Email,
            IsEmailVerified = false,
            HashedPassword = Models.User.HashPassword(input.Password),
        };

        dbContext.Users.Add(newUser);

        await dbContext.SaveChangesAsync();

        return newUser;
    }

    private async Task<Models.User> HandleSignIn(
        Contexts.AppDbContext dbContext,
        CreateSessionInput input,
        Models.User? user
    )
    {
        if (user == null)
        {
            throw new MutationException("A user with the provided email address was not found.");
        }

        var userSessionsCount = await dbContext.Sessions.CountAsync(s => s.UserId == user.Id);

        if (userSessionsCount >= MaxSessionsPerUser)
        {
            throw new MutationException(
                $"The maximum number of sessions ({MaxSessionsPerUser}) for this user has been reached."
            );
        }

        if (!Models.User.ValidatePassword(input.Password, user.HashedPassword))
        {
            throw new MutationException("The provided password is incorrect.");
        }

        return user;
    }

    /// <summary>
    /// Represents the input for the DeleteSession mutation.
    /// </summary>
    /// <param name="SessionId"></param>
    public record DeleteSessionInput(string SessionId);

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="MutationException"></exception>
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
    /// Represents the input for the CreateTempCodeForId mutation.
    /// </summary>
    /// <param name="Purpose"></param>
    /// <param name="ForId"></param>
    public record CreateTempCodeForIdInput(Models.TempCode.TempCodePurpose Purpose, string ForId);

    /// <summary>
    /// Creates a new temporary code for a specific ID.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="resend"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="MutationException"></exception>
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

        var codeSalt = Models.TempCode.GenerateCodeSalt();

        var hashedCode = Models.TempCode.HashCode(code, codeSalt);

        var message = await CreateEmailMessage(dbContext, input);

        dbContext.TempCodes.Add(
            new Models.TempCode
            {
                Id = _snowflakeGenerator.NextId().ToString(),
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

    private static string GenerateTempCode()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(TempCodeLength))[
            ..TempCodeLength
        ];
    }

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
            Models.TempCode.TempCodePurpose.ChangePassword => "Change Password",
            Models.TempCode.TempCodePurpose.ChangeEmail => "Change Email",
            Models.TempCode.TempCodePurpose.EmailVerification => "Email Verification",
            Models.TempCode.TempCodePurpose.DeleteAccount => "Removing Account",
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
}
