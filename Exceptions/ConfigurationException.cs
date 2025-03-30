namespace StreamierServer.Exceptions;

/// <summary>
/// Exception thrown when there is an error related to application configuration.
/// This typically indicates missing or invalid configuration values required for proper application operation.
/// </summary>
/// <param name="configuration">The error message describing the configuration problem.</param>
/// <remarks>
/// This exception automatically prefixes the provided message with "Configuration error: "
/// to maintain consistent error message formatting.
/// </remarks>
public class ConfigurationException(string configuration)
    : Exception($"Configuration error: {configuration}");
