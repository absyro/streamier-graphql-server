namespace Server.Exceptions;

public class ConfigurationException(string configuration)
    : Exception($"Configuration error: {configuration}");
