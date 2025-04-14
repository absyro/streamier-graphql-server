namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Custom exception type for mutation-related errors.
/// Provides consistent error handling for GraphQL mutations.
/// </summary>
/// <param name="message">The error message describing what went wrong.</param>
public class MutationException(string message) : Exception(message);
