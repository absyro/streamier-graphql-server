namespace StreamierGraphQLServer.GraphQL;

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
    /// <returns>A task that resolves to true if the email is in use, false otherwise.</returns>
    public Task<bool> IsEmailInUse([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
