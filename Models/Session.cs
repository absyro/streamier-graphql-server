namespace Server.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RandomString4Net;

/// <summary>
///  Represents a user session in the system.
/// </summary>
public class Session : Base.BaseEntity
{
    /// <summary>
    /// The ID of the user associated with this session.
    /// </summary>
    [Required]
    [Column("user_id")]
    public required string UserId { get; set; }

    /// <summary>
    /// The UTC date and time when this session will expire.
    /// </summary>
    [Required]
    [Column("expires_at")]
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Generates a cryptographically secure random session id.
    /// </summary>
    /// <returns>A base64-encoded random session token.</returns>
    public static string GenerateSessionId()
    {
        return RandomString.GetString(Types.ALPHANUMERIC_MIXEDCASE_WITH_SYMBOLS, 128);
    }
}
