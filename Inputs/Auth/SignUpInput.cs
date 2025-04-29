namespace StreamierGraphQLServer.Inputs.Auth;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input model for registering a new user account in the system.
/// </summary>
/// <remarks>
/// This input type is used in GraphQL mutations for user registration.
/// All fields are required to create a valid user account.
/// </remarks>
public class SignUpInput
{
    /// <summary>
    /// The email address that will be used as the primary account identifier.
    /// </summary>
    /// <value>
    /// A valid email address string conforming to standard email format (RFC 5322),
    /// with a maximum length of 320 characters (per RFC 3696 specification).
    /// </value>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    /// <summary>
    /// The unique username for the new user account.
    /// </summary>
    /// <value>
    /// A string between 3 and 30 characters in length.
    /// Only lowercase letters, numbers, and underscores are allowed.
    /// </value>
    [Required]
    [StringLength(30, MinimumLength = 3)]
    [RegularExpression(
        @"^[a-z0-9_]+$",
        ErrorMessage = "Username can only contain lowercase letters, numbers, and underscores."
    )]
    public required string Username { get; set; }

    /// <summary>
    /// The secret password for account authentication.
    /// </summary>
    /// <value>
    /// A secure password string between 8 and 1000 characters.
    /// For security, passwords should contain a mix of character types.
    /// </value>
    [Required]
    [StringLength(1000, MinimumLength = 8)]
    public required string Password { get; set; }
}
