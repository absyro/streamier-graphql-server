namespace StreamierGraphQLServer.GraphQL;

using System.Security.Cryptography;
using FluentValidation;
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
    /// Input type for creating a new user session.
    /// </summary>
    /// <param name="Email">The user's email address for authentication.</param>
    /// <param name="Password">The user's password for authentication.</param>
    /// <param name="Mode">The operation mode (SignUp or SignIn). Defaults to SignIn.</param>
    /// <param name="ExpirationDays">Session expiration period in days. Defaults to 30.</param>
    public record CreateSessionInput(
        string Email,
        string Password,
        CreateSessionMode Mode = CreateSessionMode.SignIn,
        int ExpirationDays = 30
    );

    /// <summary>
    /// Defines the possible modes for session creation.
    /// </summary>
    public enum CreateSessionMode
    {
        /// <summary>
        /// Creates a new user account and establishes a session.
        /// </summary>
        SignUp,

        /// <summary>
        /// Authenticates an existing user and establishes a session.
        /// </summary>
        SignIn,
    }

    /// <summary>
    /// Creates a new authentication session for a user.
    /// Handles both new user registration (SignUp) and existing user authentication (SignIn).
    /// </summary>
    /// <param name="dbContext">The database context for accessing user data.</param>
    /// <param name="validator">The validator for input validation.</param>
    /// <param name="input">The input parameters containing authentication details.</param>
    /// <returns>The newly created session object.</returns>
    /// <exception cref="MutationException">
    /// Thrown when input validation fails, authentication fails, or user creation fails.
    /// </exception>
    [Error(typeof(MutationException))]
    public async Task<Session> CreateSession(
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

        if (user is null)
        {
            throw new MutationException("Failed to find or create user.");
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
    /// Input type for deleting a session.
    /// </summary>
    /// <param name="SessionId">The ID of the session to be deleted.</param>
    public record DeleteSessionInput(string SessionId);

    /// <summary>
    /// Deletes an existing session by its ID.
    /// </summary>
    /// <param name="dbContext">The database context for session operations.</param>
    /// <param name="input">The input containing the session ID to delete.</param>
    /// <returns>true if deletion was successful.</returns>
    /// <exception cref="MutationException">
    /// Thrown when the session ID is invalid or deletion fails.
    /// </exception>
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
    /// Input type for creating a temporary code.
    /// </summary>
    /// <param name="Purpose">The purpose of the temporary code.</param>
    /// <param name="ForId">The user ID associated with the temporary code.</param>
    public record CreateTempCodeForIdInput(TempCode.TempCodePurpose Purpose, string ForId);

    /// <summary>
    /// Creates a temporary code for a specific purpose and user ID.
    /// Generates a secure code, stores its hashed version, and sends via email.
    /// </summary>
    /// <param name="dbContext">The database context for code operations.</param>
    /// <param name="resend">The email service client.</param>
    /// <param name="input">The input parameters for code generation.</param>
    /// <returns>true if code creation and email sending were successful.</returns>
    /// <exception cref="MutationException">
    /// Thrown when code already exists, user ID is invalid, or purpose is invalid.
    /// </exception>
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

    /// <summary>
    /// Generates a secure temporary code using cryptographic random number generation.
    /// </summary>
    /// <returns>A base64-encoded random string of length TempCodeLength.</returns>
    private static string GenerateTempCode()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(TempCodeLength))[
            ..TempCodeLength
        ];
    }

    /// <summary>
    /// Creates an email message for temporary code delivery.
    /// </summary>
    /// <param name="dbContext">The database context for user lookup.</param>
    /// <param name="input">The input parameters containing code purpose and user ID.</param>
    /// <returns>An <see cref="EmailMessage"/> configured for the specific code purpose.</returns>
    /// <exception cref="MutationException">
    /// Thrown when user ID is invalid or purpose is invalid.
    /// </exception>
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

    /// <summary>
    /// Handles new user registration process.
    /// </summary>
    /// <param name="dbContext">The database context for user operations.</param>
    /// <param name="input">The registration input parameters.</param>
    /// <param name="existingUser">Existing user record if found, null otherwise.</param>
    /// <returns>The newly created user.</returns>
    /// <exception cref="MutationException">
    /// Thrown when user already exists or creation fails.
    /// </exception>
    private static async Task<User> HandleSignUp(
        Contexts.AppDbContext dbContext,
        CreateSessionInput input,
        User? existingUser
    )
    {
        if (existingUser != null)
        {
            throw new MutationException("A user with the provided email address already exists.");
        }

        var newUser = new User
        {
            Id = await User.GenerateIdAsync(dbContext),
            Email = input.Email,
            IsEmailVerified = false,
            HashedPassword = User.HashPassword(input.Password),
        };

        dbContext.Users.Add(newUser);

        await dbContext.SaveChangesAsync();

        return newUser;
    }

    /// <summary>
    /// Handles existing user authentication process.
    /// </summary>
    /// <param name="dbContext">The database context for user operations.</param>
    /// <param name="input">The authentication input parameters.</param>
    /// <param name="user">The existing user record if found.</param>
    /// <returns>The authenticated user.</returns>
    /// <exception cref="MutationException">
    /// Thrown when user doesn't exist, password is invalid, or session limit reached.
    /// </exception>
    private static async Task<User> HandleSignIn(
        Contexts.AppDbContext dbContext,
        CreateSessionInput input,
        User? user
    )
    {
        if (user is null)
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

        if (!User.ValidatePassword(input.Password, user.HashedPassword))
        {
            throw new MutationException("The provided password is incorrect.");
        }

        return user;
    }
}
