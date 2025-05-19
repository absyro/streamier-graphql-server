namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Exception thrown when there is an error related to application configuration.
/// This typically indicates missing or invalid configuration values required for proper application operation.
/// </summary>
public class ConfigurationException(string configuration)
    : Exception($"Configuration error: {configuration}");
