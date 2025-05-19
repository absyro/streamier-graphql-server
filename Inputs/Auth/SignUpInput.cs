namespace StreamierGraphQLServer.Inputs.Auth;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input model for registering a new user account in the system.
/// </summary>
public class SignUpInput
{
    /// <summary>
    /// The email address that will be used as the primary account identifier.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    /// <summary>
    /// The unique username for the new user account.
    /// </summary>
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
    [Required]
    [StringLength(1000, MinimumLength = 8)]
    public required string Password { get; set; }
}
