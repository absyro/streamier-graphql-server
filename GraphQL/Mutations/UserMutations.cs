namespace StreamierGraphQLServer.GraphQL.Mutations;

using Microsoft.EntityFrameworkCore;
using StreamierGraphQLServer.Contexts;
using StreamierGraphQLServer.GraphQL.Context;
using StreamierGraphQLServer.Inputs.User;
using StreamierGraphQLServer.Models;

/// <summary>
/// Contains mutations related to user profile management.
/// </summary>
[ExtendObjectType("Mutation")]
public class UserMutations
{
    /// <summary>
    /// Updates the current user's profile information.
    /// </summary>
    public async Task<User> UpdateUser(
        [Service] AppDbContext dbContext,
        [Service] GraphQLContext graphQLContext,
        UpdateUserInput input
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

        var user = await dbContext.Users.FirstOrDefaultAsync(u =>
            u.Sessions.Any(s => s.Id == sessionId)
        );

        if (user == null)
        {
            throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );
        }

        if (!string.IsNullOrEmpty(input.Bio))
        {
            user.Bio = input.Bio;
        }

        user.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return user;
    }
}
