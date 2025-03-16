namespace Server.GraphQL;

using Microsoft.EntityFrameworkCore;

public class Query
{
    [UseProjection]
    [UseFirstOrDefault]
    [GraphQLDescription("This query returns a user which is associated with the provided session.")]
    public IQueryable<Models.User>? GetUser(
        [Service] Contexts.AppDbContext dbContext,
        string sessionId
    )
    {
        var session = dbContext
            .Sessions.Where(session => session.Id == sessionId)
            .Select(session => new { session.UserId })
            .FirstOrDefault();

        if (session == null)
        {
            return null;
        }

        return dbContext.Users.Where(user => user.Id == session.UserId);
    }

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [GraphQLDescription(
        "This query returns the sessions of a user. The user's session ID must be provided."
    )]
    public IQueryable<Models.Session>? GetUserSessions(
        [Service] Contexts.AppDbContext dbContext,
        string sessionId
    )
    {
        var session = dbContext
            .Sessions.Where(session => session.Id == sessionId)
            .Select(session => new { session.UserId })
            .FirstOrDefault();

        if (session == null)
        {
            return null;
        }

        return dbContext.Sessions.Where(s => s.UserId == session.UserId);
    }

    [GraphQLDescription("This query returns whether or not a user exists.")]
    public async Task<bool> DoesUserExist([Service] Contexts.AppDbContext dbContext, string email)
    {
        return await dbContext.Users.AnyAsync(user => user.Email == email);
    }
}
