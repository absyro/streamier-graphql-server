namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Input for enabling two-factor authentication
/// </summary>
public class EnableTwoFactorAuthenticationInput
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
}
