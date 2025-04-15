namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when an invalid password is provided.
/// </summary>
public class InvalidPasswordException() : Exception("The provided password is invalid.");
