namespace StreamierGraphQLServer.GraphQL;

using System.Threading.Tasks;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using StreamierGraphQLServer.Models.Base;
using StreamierGraphQLServer.Models.Users;

/// <summary>
/// GraphQL query class containing all the root query operations.
/// </summary>
public class Query
{
    /// <summary>
    /// Retrieves a single user based on their session ID.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="sessionId">The session ID to look up.</param>
    /// <returns>An <see cref="IQueryable"/> of <see cref="User"/> that can be further filtered or projected.</returns>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<User> GetUser([Service] Contexts.AppDbContext dbContext, string sessionId)
    {
        return dbContext.Users.Where(u => u.Sessions.Any(s => s.Id == sessionId));
    }

    /// <summary>
    /// Nested class representing a user profile with basic information.
    /// </summary>
    public class UserProfile : BaseEntity
    {
        /// <summary>
        /// The biography or description of the user.
        /// </summary>
        public required string Bio { get; set; }
    }

    /// <summary>
    /// Retrieves a user profile for a specific user ID.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="userId">The ID of the user to fetch the profile for.</param>
    /// <returns>An <see cref="IQueryable"/> of <see cref="UserProfile"/> that can be further filtered or projected.</returns>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<UserProfile> GetUserProfile(
        [Service] Contexts.AppDbContext dbContext,
        string userId
    )
    {
        return dbContext
            .Users.Where(u => u.Id == userId)
            .Select(u => new UserProfile
            {
                Id = u.Id,
                Bio = u.Bio,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
            });
    }

    /// <summary>
    /// Checks if an email address is already in use by any user.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the email is in use, false otherwise.</returns>
    public Task<bool> IsEmailInUse([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// Checks if a username is already in use by any user.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the username is in use, false otherwise.</returns>
    public Task<bool> IsUsernameInUse([Service] Contexts.AppDbContext dbContext, string username)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Username == username);
    }

    /// <summary>
    /// Checks if an account has two-factor authentication enabled.
    /// Requires password verification for security.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="emailOrUsername">The email or username of the account to check.</param>
    /// <param name="password">The account password for verification.</param>
    /// <returns>
    /// Boolean indicating if 2FA is enabled.
    /// </returns>
    /// <exception cref="GraphQLException">Thrown when the user is not found or password is invalid.</exception>
    public async Task<bool> IsTwoFactorAuthenticationEnabled(
        [Service] Contexts.AppDbContext dbContext,
        string emailOrUsername,
        string password
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u =>
            u.Email == emailOrUsername || u.Username == emailOrUsername
        );

        if (user == null)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Account not found.")
                    .SetCode("ACCOUNT_NOT_FOUND")
                    .Build()
            );
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Invalid credentials.")
                    .SetCode("INVALID_CREDENTIALS")
                    .Build()
            );
        }

        return user.TwoFactorAuthentication != null;
    }
}
