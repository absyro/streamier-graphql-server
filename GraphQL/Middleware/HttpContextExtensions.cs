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
    public const string AuthorizationHeaderName = "Authorization";

    /// <summary>
    /// The authentication scheme used for session authentication.
    /// </summary>
    public const string SessionAuthScheme = "Session";

    /// <summary>
    /// Gets the session ID from the HTTP request headers.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The session ID if present in the headers, otherwise null.</returns>
    public static string? GetSessionId(this HttpContext context)
    {
        if (
            context.Request.Headers.TryGetValue(AuthorizationHeaderName, out var authHeader)
            && !string.IsNullOrEmpty(authHeader)
        )
        {
            string authValue = authHeader.ToString();

            if (authValue.StartsWith($"{SessionAuthScheme} ", StringComparison.OrdinalIgnoreCase))
            {
                return authValue[(SessionAuthScheme.Length + 1)..].Trim();
            }
        }

        return null;
    }
}
