namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents an authenticated user session within the application, tracking active login sessions
/// for security, analytics, and session management purposes.
/// </summary>
[Table("user_sessions")]
public class UserSession : BaseEntity
{
    /// <summary>
    /// The UTC date and time when this session becomes invalid.
    /// After this time, the session token cannot be used for authentication.
    /// </summary>
    [Required]
    public required DateTime ExpiresAt { get; set; }
}
