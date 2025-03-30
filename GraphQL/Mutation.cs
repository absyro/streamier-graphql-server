namespace Server.GraphQL;

using System.Security.Cryptography;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resend;
using Snowflake.Net;

public class Mutation
{
    private readonly IdWorker _snowflakeGenerator = new(1, 1);

    public class MutationException(string message) : Exception(message);

    public class CreateSessionInput
    {
        public required string Email { get; set; }

        public required string Password { get; set; }

        public CreateSessionMode Mode { get; set; }

        public int ExpirationDays { get; set; }
    }

    public enum CreateSessionMode
    {
        SignUp,
        SignIn,
    }

    [Error(typeof(MutationException))]
    [GraphQLDescription(
        "This mutation creates a new session. This method is also used for signing in and signing up."
    )]
    public async Task<Models.Session> CreateSession(
        [Service] Contexts.AppDbContext dbContext,
        [Service] IValidator<CreateSessionInput> validator,
        CreateSessionInput input
    )
    {
        var validationResult = await validator.ValidateAsync(input);

        if (!validationResult.IsValid)
        {
            throw new MutationException(validationResult.Errors.First().ErrorMessage);
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == input.Email);

        switch (input.Mode)
        {
            case CreateSessionMode.SignUp:
            {
                if (user != null)
                {
                    throw new MutationException(
                        "A user with the provided email address already exists."
                    );
                }

                user = new Models.User
                {
                    Id = _snowflakeGenerator.NextId().ToString(),
                    Email = input.Email,
                    IsEmailVerified = false,
                    HashedPassword = Models.User.HashPassword(input.Password),
                };

                dbContext.Users.Add(user);

                await dbContext.SaveChangesAsync();

                break;
            }

            case CreateSessionMode.SignIn:
            {
                if (user == null)
                {
                    throw new MutationException(
                        "A user with the provided email address was not found."
                    );
                }

                var userSessionsCount = await dbContext.Sessions.CountAsync(session =>
                    session.UserId == user.Id
                );

                if (userSessionsCount >= 20)
                {
                    throw new MutationException(
                        "The maximum number of sessions for this user has been reached."
                    );
                }

                if (!Models.User.ValidatePassword(input.Password, user.HashedPassword))
                {
                    throw new MutationException("The provided password is incorrect.");
                }

                break;
            }
        }

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

    public class DeleteSessionInput
    {
        public required string SessionId { get; set; }
    }

    [Error(typeof(MutationException))]
    [GraphQLDescription("This mutation deletes a session.")]
    public async Task<bool> DeleteSession(
        [Service] Contexts.AppDbContext dbContext,
        DeleteSessionInput input
    )
    {
        var session =
            await dbContext.Sessions.FirstOrDefaultAsync(session => session.Id == input.SessionId)
            ?? throw new MutationException("Invalid session ID.");

        dbContext.Sessions.Remove(session);

        await dbContext.SaveChangesAsync();

        return true;
    }

    public class CreateTempCodeForIdInput
    {
        public Models.TempCode.TempCodePurpose Purpose { get; set; }

        public required string ForId { get; set; }
    }

    [Error(typeof(MutationException))]
    [GraphQLDescription("This mutation creates a temp code for the given entity ID.")]
    public async Task<bool> CreateTempCodeForId(
        [Service] Contexts.AppDbContext dbContext,
        [Service] IResend resend,
        CreateTempCodeForIdInput input
    )
    {
        var isTempCodeRegistered = await dbContext.TempCodes.AnyAsync(tempCode =>
            tempCode.Purpose == input.Purpose && tempCode.ForId == input.ForId
        );

        if (isTempCodeRegistered)
        {
            throw new MutationException(
                "Same code with the same purpose has already been registered for the same ID."
            );
        }

        var code = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16))[..16];

        var codeSalt = Models.TempCode.GenerateCodeSalt();

        var hashedCode = Models.TempCode.HashCode(code, codeSalt);

        var message = new EmailMessage { From = "core@botstudioo.com" };

        switch (input.Purpose)
        {
            case Models.TempCode.TempCodePurpose.ChangePassword:
            {
                var user =
                    await dbContext
                        .Users.Where(user => user.Id == input.ForId)
                        .Select(user => new { user.Email })
                        .FirstOrDefaultAsync() ?? throw new MutationException("Invalid user ID.");

                message.To.Add(user.Email);

                message.Subject = "Change Password";

                message.HtmlBody = "<strong>it works!</strong>";

                break;
            }

            case Models.TempCode.TempCodePurpose.ChangeEmail:
            {
                var user =
                    await dbContext
                        .Users.Where(user => user.Id == input.ForId)
                        .Select(user => new { user.Email })
                        .FirstOrDefaultAsync() ?? throw new MutationException("Invalid user ID.");

                message.To.Add(user.Email);

                message.Subject = "Change Email";

                message.HtmlBody = "<strong>it works!</strong>";

                break;
            }

            case Models.TempCode.TempCodePurpose.EmailVerification:
            {
                var user =
                    await dbContext
                        .Users.Where(user => user.Id == input.ForId)
                        .Select(user => new { user.Email })
                        .FirstOrDefaultAsync() ?? throw new MutationException("Invalid user ID.");

                message.To.Add(user.Email);

                message.Subject = "Email Verification";

                message.HtmlBody = "<strong>it works!</strong>";

                break;
            }

            case Models.TempCode.TempCodePurpose.DeleteAccount:
            {
                var user =
                    await dbContext
                        .Users.Where(user => user.Id == input.ForId)
                        .Select(user => new { user.Email })
                        .FirstOrDefaultAsync() ?? throw new MutationException("Invalid user ID.");

                message.To.Add(user.Email);

                message.Subject = "Removing Account";

                message.HtmlBody = "<strong>it works!</strong>";

                break;
            }
        }

        dbContext.TempCodes.Add(
            new Models.TempCode
            {
                Id = _snowflakeGenerator.NextId().ToString(),
                Purpose = input.Purpose,
                ForId = input.ForId,
                HashedCode = hashedCode,
                CodeSalt = codeSalt,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
            }
        );

        await dbContext.SaveChangesAsync();

        await resend.EmailSendAsync(message);

        return true;
    }
}
