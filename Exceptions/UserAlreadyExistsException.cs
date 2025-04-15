namespace StreamierGraphQLServer.Exceptions;

/// <summary>
/// Thrown when a user with the provided email already exists during sign up.
/// </summary>
/// <param name="email">The email address of the existing user.</param>
public class UserAlreadyExistsException(string email)
    : Exception($"A user with the email ({email}) already exists.");
