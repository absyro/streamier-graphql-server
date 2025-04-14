namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when an invalid user ID is provided for temp code generation.
/// </summary>
/// <param name="userId">The invalid user ID.</param>
public class InvalidUserIdException(string userId) : Exception($"Invalid user ID: {userId}");
