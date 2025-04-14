namespace StreamierGraphQLServer.GraphQL;

using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
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
        return dbContext
            .Users.Include(u => u.Sessions)
            .Where(u => u.Sessions.Any(s => s.Id == sessionId));
    }

    /// <summary>
    /// Retrieves all sessions associated with a user identified by a session ID.
    /// Supports paging, projection, and filtering.
    /// </summary>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    public IQueryable<UserSession>? GetUserSessions(
        [Service] Contexts.AppDbContext dbContext,
        string sessionId
    )
    {
        var user = dbContext
            .Users.Include(u => u.Sessions)
            .AsNoTracking()
            .FirstOrDefault(u => u.Sessions.Any(s => s.Id == sessionId));

        return user?.Sessions.AsQueryable();
    }

    /// <summary>
    /// Checks whether a user with the specified email exists in the system.
    /// </summary>
    public Task<bool> DoesUserExist([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
