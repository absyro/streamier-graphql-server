namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when a password doesn't meet the required strength/complexity criteria.
/// </summary>
public class WeakPasswordException()
    : Exception("The provided password doesn't meet the required strength criteria.");
