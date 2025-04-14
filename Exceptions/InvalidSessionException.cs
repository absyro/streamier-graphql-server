namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when an invalid session ID is provided for deletion.
/// </summary>
/// <param name="sessionId">The invalid session ID.</param>
public class InvalidSessionException(string sessionId)
    : Exception($"Invalid session ID: {sessionId}");
