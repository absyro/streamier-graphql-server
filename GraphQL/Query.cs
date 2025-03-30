namespace Server.GraphQL;

using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

public class Query
{
    [UseProjection]
    [UseFirstOrDefault]
    [GraphQLDescription("Returns a user associated with the provided session ID")]
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

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [GraphQLDescription("Returns paginated sessions for a user identified by session ID")]
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

    [GraphQLDescription("Checks if a user with the specified email exists")]
    public Task<bool> DoesUserExist([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
