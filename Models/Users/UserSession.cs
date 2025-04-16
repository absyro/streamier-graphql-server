namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents an authenticated user session within the application, tracking active login sessions
/// for security, analytics, and session management purposes.
/// </summary>
/// <remarks>
/// Sessions are automatically invalidated when they expire or when the user logs out.
/// Multiple concurrent sessions are supported per user account.
/// </remarks>
[Table("user_sessions")]
public class UserSession : Base.BaseEntity
{
    /// <summary>
    /// The UTC date and time when this session becomes invalid.
    /// After this time, the session token cannot be used for authentication.
    /// </summary>
    [Required]
    public required DateTime ExpiresAt { get; set; }
}
