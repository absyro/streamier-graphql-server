namespace StreamierServer.GraphQL;

using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the root GraphQL query type containing all available query operations.
/// </summary>
public class Query
{
    /// <summary>
    /// Retrieves a single user by their session ID.
    /// </summary>
    /// <param name="dbContext">The database context for accessing user data.</param>
    /// <param name="sessionId">The session ID used to identify the user.</param>
    /// <returns>
    /// An <see cref="IQueryable"/> of <see cref="Models.User"/> or <c>null</c> if no user is found for the given session ID.
    /// The queryable allows for additional filtering and projection at the GraphQL level.
    /// </returns>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<Models.User>? GetUser(
        [Service] Contexts.AppDbContext dbContext,
        string sessionId
    )
    {
        var userId = dbContext
            .Sessions.AsNoTracking()
            .Where(s => s.Id == sessionId)
            .Select(s => s.UserId)
            .FirstOrDefault();

        return string.IsNullOrEmpty(userId) ? null : dbContext.Users.Where(u => u.Id == userId);
    }

    /// <summary>
    /// Retrieves all sessions associated with a user identified by a session ID.
    /// Supports paging, projection, and filtering.
    /// </summary>
    /// <param name="dbContext">The database context for accessing session data.</param>
    /// <param name="sessionId">The session ID used to identify the user.</param>
    /// <returns>
    /// An <see cref="IQueryable"/> of <see cref="Models.Session"/> or <c>null</c> if no user is found for the given session ID.
    /// The result supports paging, filtering, and projection at the GraphQL level.
    /// </returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    public IQueryable<Models.Session>? GetUserSessions(
        [Service] Contexts.AppDbContext dbContext,
        string sessionId
    )
    {
        var userId = dbContext
            .Sessions.AsNoTracking()
            .Where(s => s.Id == sessionId)
            .Select(s => s.UserId)
            .FirstOrDefault();

        return string.IsNullOrEmpty(userId)
            ? null
            : dbContext.Sessions.Where(s => s.UserId == userId);
    }

    /// <summary>
    /// Checks whether a user with the specified email exists in the system.
    /// </summary>
    /// <param name="dbContext">The database context for accessing user data.</param>
    /// <param name="email">The email address to check for existence.</param>
    /// <returns>
    /// A <c>Task</c> that resolves to <c>true</c> if a user with the email exists, <c>false</c> otherwise.
    /// </returns>
    public Task<bool> DoesUserExist([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
