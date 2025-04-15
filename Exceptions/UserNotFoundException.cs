namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when a user is not found.
/// </summary>
public class UserNotFoundException() : Exception("User not found.");
