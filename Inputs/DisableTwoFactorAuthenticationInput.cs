namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Input for disabling two-factor authentication.
/// </summary>
public class DisableTwoFactorAuthenticationInput
{
    /// <summary>
    /// The user's current password for verification
    /// </summary>
    /// <value>
    /// A non-nullable string representing the user's password.
    /// </value>
    [Required]
    [StringLength(1000, MinimumLength = 8)]
    public required string Password { get; set; }
}
