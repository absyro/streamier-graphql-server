namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when an invalid temp code purpose is provided.
/// </summary>
/// <param name="purpose">The invalid temp code purpose.</param>
public class InvalidTempCodePurposeException(string purpose)
    : Exception($"Invalid temp code purpose: {purpose}");
