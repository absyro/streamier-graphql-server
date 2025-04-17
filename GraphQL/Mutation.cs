namespace StreamierGraphQLServer.GraphQL;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
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
    /// <param name="dbContext">The application database context.</param>
    /// <param name="input">User registration data including email and password.</param>
    /// <returns>The newly created User object.</returns>
    /// <exception cref="ValidationFailedException">Thrown when input validation fails.</exception>
    /// <exception cref="EmailAlreadyExistsException">Thrown when the email is already registered.</exception>
    /// <exception cref="UsernameTakenException">Thrown when the username is already taken.</exception>
    /// <exception cref="WeakPasswordException">Thrown when the password doesn't meet strength requirements.</exception>
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
    /// <param name="dbContext">The application database context.</param>
    /// <param name="input">Login credentials including email, password, and desired session expiration.</param>
    /// <returns>The newly created <see cref="UserSession"/> object.</returns>
    /// <exception cref="ValidationFailedException">Thrown when input validation fails.</exception>
    /// <exception cref="UserNotFoundException">Thrown when no user exists with the provided email.</exception>
    /// <exception cref="InvalidPasswordException">Thrown when password verification fails.</exception>
    /// <exception cref="InvalidSessionExpirationException">Thrown when expiration date is outside allowed range.</exception>
    /// <exception cref="MaxSessionsExceededException">Thrown when user already has maximum allowed sessions.</exception>
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

        var now = DateTime.UtcNow;

        var minExpiration = now.AddHours(1);
        var maxExpiration = now.AddDays(365);

        if (input.ExpirationDate < minExpiration || input.ExpirationDate > maxExpiration)
        {
            throw new InvalidSessionExpirationException(minExpiration, maxExpiration);
        }

        const int MaxSessionsPerUser = 3;

        var userSessionsCount = await dbContext
            .Users.Where(u => u.Id == user.Id)
            .Select(u => u.Sessions.Count)
            .FirstOrDefaultAsync();

        if (userSessionsCount >= MaxSessionsPerUser)
        {
            throw new MaxSessionsExceededException(MaxSessionsPerUser);
        }

        var session = new UserSession
        {
            Id = RandomString.GetString(Types.ALPHANUMERIC_MIXEDCASE_WITH_SYMBOLS, 128),
            ExpiresAt = input.ExpirationDate,
        };

        user.Sessions.Add(session);

        await dbContext.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Terminates an existing authentication session.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="input">Session deletion request containing the session ID.</param>
    /// <returns>True if the session was successfully deleted.</returns>
    /// <exception cref="ValidationFailedException">Thrown when input validation fails.</exception>
    /// <exception cref="InvalidSessionException">Thrown when the session doesn't exist or is invalid.</exception>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(InvalidSessionException))]
    public async Task<bool> DeleteSession(
        [Service] Contexts.AppDbContext dbContext,
        DeleteSessionInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext
                .Users.Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.Sessions.Any(s => s.Id == input.SessionId))
            ?? throw new InvalidSessionException(input.SessionId);

        var session =
            user.Sessions.FirstOrDefault(s => s.Id == input.SessionId)
            ?? throw new InvalidSessionException(input.SessionId);

        user.Sessions.Remove(session);

        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="input">Update request containing the changes and session ID for authentication.</param>
    /// <returns>The updated User object.</returns>
    /// <exception cref="ValidationFailedException">Thrown when input validation fails.</exception>
    /// <exception cref="UserNotFoundException">Thrown when no valid user session exists.</exception>
    [Error(typeof(ValidationFailedException))]
    [Error(typeof(UserNotFoundException))]
    public async Task<User> UpdateUser(
        [Service] Contexts.AppDbContext dbContext,
        UpdateUserInput input
    )
    {
        ValidateInput(input);

        var user =
            await dbContext
                .Users.Where(u => u.Sessions.Any(s => s.Id == input.SessionId))
                .FirstOrDefaultAsync() ?? throw new UserNotFoundException();

        if (input.Bio != null)
        {
            user.Bio = input.Bio;
        }

        await dbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Validates an input object using its data annotation attributes.
    /// </summary>
    /// <param name="input">The input object to validate.</param>
    /// <exception cref="ValidationFailedException">Thrown when validation fails.</exception>
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
