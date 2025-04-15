namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when a user is not found.
/// </summary>
/// <param name="email">The email address of the non-existent user.</param>
public class UserNotFoundException(string email)
    : Exception($"No user found with email ({email}).");
