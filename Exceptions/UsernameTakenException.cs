namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when the username is already taken.
/// </summary>
/// <param name="username">The username that is already taken.</param>
public class UsernameTakenException(string username)
    : Exception($"The username ({username}) is already taken.");
