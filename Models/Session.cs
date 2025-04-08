namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using RandomString4Net;

/// <summary>
/// Represents an authenticated user session within the application.
/// </summary>
/// <remarks>
/// This entity tracks active user sessions, including their validity period
/// and association with specific users. Sessions are protected by cryptographically
/// strong identifiers.
/// </remarks>
public class Session : Base.BaseEntity
{
    /// <summary>
    /// The unique identifier of the user associated with this session.
    /// </summary>
    /// <value>
    /// A string representing the user's unique ID. This field is required.
    /// </value>
    [Required]
    [StringLength(8)]
    public required string UserId { get; set; }

    /// <summary>
    /// The UTC date and time when this session becomes invalid.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> in UTC format indicating when the session will expire.
    /// After this time, the session token cannot be used for authentication.
    /// This field is required.
    /// </value>
    [Required]
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Generates a cryptographically secure session identifier.
    /// </summary>
    /// <returns>
    /// A 128-character random string containing alphanumeric characters
    /// (mixed case) and symbols, suitable for use as a session token.
    /// </returns>
    /// <remarks>
    /// This method uses <see cref="RandomString"/> to generate
    /// high-entropy session tokens that are resistant to brute force attacks.
    /// </remarks>
    public static string GenerateSessionId()
    {
        return RandomString.GetString(Types.ALPHANUMERIC_MIXEDCASE_WITH_SYMBOLS, 128);
    }
}
