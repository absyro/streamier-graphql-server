namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Exception thrown when an invalid two-factor authentication code is provided.
/// </summary>
public class InvalidTwoFactorAuthenticationCodeException()
    : Exception("Invalid two-factor authentication code.");
