namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when an invalid password is provided during sign in.
/// </summary>
public class InvalidPasswordException() : Exception("The provided password is incorrect.");
