namespace StreamierGraphQLServer.GraphQL.Middleware;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Extension methods for accessing GraphQL contextual information from HttpContext.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// The header name used for session authentication.
    /// </summary>
    public const string SessionHeaderName = "X-Session-Id";

    /// <summary>
    /// Gets the session ID from the HTTP request headers.
    /// </summary>
    public static string? GetSessionId(this HttpContext context)
    {
        if (
            context.Request.Headers.TryGetValue(SessionHeaderName, out var sessionHeader)
            && !string.IsNullOrEmpty(sessionHeader)
        )
        {
            return sessionHeader.ToString().Trim();
        }

        return null;
    }
}
