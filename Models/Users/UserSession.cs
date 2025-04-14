namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RandomString4Net;

/// <summary>
/// Represents an authenticated user session within the application.
/// </summary>
[Table("user_sessions")]
public class UserSession : Base.BaseEntity
{
    /// <summary>
    /// The UTC date and time when this session becomes invalid.
    /// </summary>
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
