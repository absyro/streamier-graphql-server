namespace StreamierGraphQLServer.GraphQL;

using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using StreamierGraphQLServer.Models;
using StreamierGraphQLServer.Models.Users;

/// <summary>
/// Represents the root GraphQL query type containing all available query operations.
/// </summary>
public class Query
{
    /// <summary>
    /// Retrieves a single user by their session ID.
    /// </summary>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<User>? GetUser([Service] Contexts.AppDbContext dbContext, string sessionId)
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
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    public IQueryable<Session>? GetUserSessions(
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
    public Task<bool> DoesUserExist([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
