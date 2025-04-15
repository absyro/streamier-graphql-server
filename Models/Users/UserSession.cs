namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}
