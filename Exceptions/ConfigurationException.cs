namespace StreamierServer.Exceptions;

/// <summary>
/// Represents a configuration exception.
/// </summary>
/// <param name="configuration">The configuration error message.</param>
public class ConfigurationException(string configuration)
    : Exception($"Configuration error: {configuration}");
