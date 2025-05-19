namespace StreamierGraphQLServer.GraphQL.Middleware;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Middleware that extracts the session ID from HTTP headers and adds it to the GraphQL request context.
/// </summary>
public class SessionHeaderMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Processes the HTTP request to extract session information from headers.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var sessionId = context.GetSessionId();

        if (sessionId != null)
        {
            context.Items["SessionId"] = sessionId;
        }

        await next(context);
    }
}
