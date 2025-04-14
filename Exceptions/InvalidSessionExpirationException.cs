namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when session expiration date is outside valid range.
/// </summary>
/// <param name="min">The minimum valid expiration date.</param>
/// <param name="max">The maximum valid expiration date.</param>
public class InvalidSessionExpirationException(DateTime min, DateTime max)
    : Exception($"Expiration must be between {min} and {max}.");
