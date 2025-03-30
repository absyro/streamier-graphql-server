namespace Server.GraphQL;

using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the GraphQL query operations.
/// </summary>
public class Query
{
    /// <summary>
    /// Returns a user identified by session ID
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="sessionId"></param>
    /// <returns>The user</returns>
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
    /// Returns a list of sessions for the user identified by session ID
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="sessionId"></param>
    /// <returns>A list of sessions</returns>
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
    /// Checks if a user exists
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="email"></param>
    /// <returns>True if the user exists</returns>
    public Task<bool> DoesUserExist([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
