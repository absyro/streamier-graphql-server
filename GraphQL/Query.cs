namespace StreamierGraphQLServer.GraphQL;

using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using StreamierGraphQLServer.Models.Users;

public class Query
{
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<User> GetUser([Service] Contexts.AppDbContext dbContext, string sessionId)
    {
        return dbContext.Users.Where(u => u.Sessions.Any(s => s.Id == sessionId));
    }

    public class UserProfile
    {
        public required string Id { get; set; }

        public string? Bio { get; set; }
    }

    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<UserProfile> GetUserProfile(
        [Service] Contexts.AppDbContext dbContext,
        string userId
    )
    {
        return dbContext
            .Users.Where(u => u.Id == userId)
            .Select(u => new UserProfile { Id = u.Id, Bio = u.Bio });
    }

    public Task<bool> IsEmailInUse([Service] Contexts.AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
