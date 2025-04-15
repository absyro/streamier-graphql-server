namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when a temporary code already exists for the same purpose and ID.
/// </summary>
/// <param name="purpose">The purpose of the temporary code.</param>
/// <param name="entityId">The ID of the entity associated with the temporary code.</param>
public class TempCodeAlreadyExistsException(string purpose, string entityId)
    : Exception(
        $"A temporary code for purpose ({purpose}) already exists for entity ID ({entityId})"
    );
