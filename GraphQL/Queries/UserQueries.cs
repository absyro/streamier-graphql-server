namespace StreamierGraphQLServer.GraphQL.Queries;

using System.Threading.Tasks;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using StreamierGraphQLServer.Contexts;
using StreamierGraphQLServer.GraphQL.Context;
using StreamierGraphQLServer.GraphQL.Types;
using StreamierGraphQLServer.Models;

/// <summary>
/// GraphQL queries related to user operations.
/// </summary>
[ExtendObjectType("Query")]
public class UserQueries
{
    /// <summary>
    /// Retrieves the current user based on the session ID from the request context.
    /// </summary>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<User> GetUser(
        [Service] AppDbContext dbContext,
        [Service] GraphQLContext graphQLContext
    )
    {
        var sessionId = graphQLContext.GetSessionId();

        if (string.IsNullOrEmpty(sessionId))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Session ID is required")
                    .SetCode("SESSION_REQUIRED")
                    .Build()
            );
        }

        return dbContext.Users.Where(u => u.Sessions.Any(s => s.Id == sessionId));
    }

    /// <summary>
    /// Retrieves a user profile for a specific user ID.
    /// </summary>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<UserProfile> GetUserProfile([Service] AppDbContext dbContext, string userId)
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
    public Task<bool> IsEmailInUse([Service] AppDbContext dbContext, string email)
    {
        return dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
