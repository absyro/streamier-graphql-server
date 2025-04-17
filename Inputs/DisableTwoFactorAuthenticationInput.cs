namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Input for disabling two-factor authentication
/// </summary>
public class DisableTwoFactorAuthenticationInput
{
    /// <summary>
    /// The session ID of the authenticated user
    /// </summary>
    /// <value>
    /// A non-nullable string representing the session's unique key.
    /// </value>
    [Required]
    [StringLength(128)]
    public required string SessionId { get; set; }

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
