namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents a user activity or interaction.
/// </summary>
[Table("user_activities")]
public class UserActivity : Base.BaseEntity
{
    /// <summary>
    /// Type of user activity.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// User logged into the system
        /// </summary>
        Login,

        /// <summary>
        /// User created a new stream
        /// </summary>
        StreamCreated,

        /// <summary>
        /// User followed another user
        /// </summary>
        Follow,

        /// <summary>
        /// User unfollowed a previously followed user
        /// </summary>
        Unfollow,

        /// <summary>
        /// User updated their profile information
        /// </summary>
        ProfileUpdate,

        /// <summary>
        /// User changed their account password
        /// </summary>
        PasswordChange,
    }

    /// <summary>
    /// Type of activity (e.g., Login, StreamCreated, Follow).
    /// </summary>
    [Required]
    public required ActivityType Type { get; set; }

    /// <summary>
    /// Detailed description of the activity.
    /// </summary>
    [Required]
    public required string Description { get; set; }
}
