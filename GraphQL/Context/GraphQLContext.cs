namespace StreamierGraphQLServer.GraphQL.Context;

using Microsoft.AspNetCore.Http;
using StreamierGraphQLServer.GraphQL.Middleware;

/// <summary>
/// Provides access to the current GraphQL request context.
/// </summary>
public class GraphQLContext(IHttpContextAccessor httpContextAccessor)
{
    /// <summary>
    /// Gets the session ID from the current HTTP context.
    /// </summary>
    public string? GetSessionId()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            return null;
        }

        if (httpContext.Items.TryGetValue("SessionId", out var sessionId) && sessionId is string id)
        {
            return id;
        }

        return httpContext.GetSessionId();
    }
}
