namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input required for user sign-in.
/// </summary>
public class SignInInput
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// The password for the user account.
    /// </summary>
    [Required]
    public required string Password { get; set; }

    /// <summary>
    /// The expiration date for the authentication session.
    /// </summary>
    [Required]
    public required DateTime ExpirationDate { get; set; }
}
