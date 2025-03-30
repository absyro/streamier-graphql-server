namespace Server.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

/// <summary>
///  Represents a user session in the system.
/// </summary>
public class Session : Base.BaseEntity
{
    /// <summary>
    ///  The size of the session token in bytes.
    /// </summary>
    private const int SessionTokenSize = 128;

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
    /// Generates a cryptographically secure random session token.
    /// </summary>
    /// <returns>A base64-encoded random session token.</returns>
    public static string GenerateRandomSessionToken()
    {
        var tokenBytes = new byte[SessionTokenSize];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(tokenBytes);

        return Convert.ToBase64String(tokenBytes);
    }
}
