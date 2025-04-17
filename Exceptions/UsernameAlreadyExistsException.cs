namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when a user with the provided username already exists.
/// </summary>
/// <param name="username">The username of the existing user.</param>
public class UsernameAlreadyExistsException(string username)
    : Exception($"A user with the username ({username}) already exists.");
