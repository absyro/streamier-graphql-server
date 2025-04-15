namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input required for user sign-up.
/// </summary>
public class SignUpInput
{
    /// <summary>
    /// The email address for the new user account.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    /// <summary>
    /// The password for the new user account.
    /// </summary>
    [Required]
    [StringLength(1000, MinimumLength = 8)]
    public required string Password { get; set; }

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
}
