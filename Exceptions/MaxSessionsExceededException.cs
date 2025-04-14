namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when maximum number of sessions per user is exceeded.
/// </summary>
/// <param name="max">The maximum number of sessions allowed per user.</param>
public class MaxSessionsExceededException(int max)
    : Exception($"Maximum number of sessions ({max}) has been reached.");
