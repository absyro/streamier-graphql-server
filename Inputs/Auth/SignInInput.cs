namespace StreamierGraphQLServer.Inputs.Auth;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the authentication credentials and session parameters for user login.
/// </summary>
public class SignInInput
{
    /// <summary>
    /// The email address associated with the user account.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    /// <summary>
    /// The secret password for account verification.
    /// </summary>
    [Required]
    [StringLength(1000, MinimumLength = 8)]
    public required string Password { get; set; }

    /// <summary>
    /// The two-factor authentication code provided by the user.
    /// </summary>
    public string? TwoFactorAuthenticationCode { get; set; }

    /// <summary>
    /// The termination timestamp for the authentication session.
    /// </summary>
    [Required]
    public required DateTime ExpirationDate { get; set; }
}
