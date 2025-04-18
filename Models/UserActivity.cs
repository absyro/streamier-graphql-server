namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents an auditable record of user actions and system interactions within the platform.
/// This entity is persisted to the 'user_activities' table for historical tracking and analytics.
/// </summary>
/// <remarks>
/// Activities are used for security auditing, user behavior analysis, and generating activity feeds.
/// </remarks>
[Table("user_activities")]
public class UserActivity : BaseEntity
{
    /// <summary>
    /// The classification of this activity record.
    /// Determines how the activity is processed and displayed.
    /// </summary>
    [Required]
    public required ActivityType Type { get; set; }

    /// <summary>
    /// Additional structured data associated with the activity.
    /// Stored in JSON format to accommodate varying activity-specific information.
    /// </summary>
    [GraphQLType(typeof(JsonType))]
    [Column(TypeName = "jsonb")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Enumerates the types of user activities tracked by the system.
    /// Each value corresponds to a significant user action or system event.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Records when a user successfully authenticates into the system.
        /// Includes both initial login and session renewals.
        /// </summary>
        Login,

        /// <summary>
        /// Created when a user initiates a new streaming session.
        /// </summary>
        StreamCreated,

        /// <summary>
        /// Generated when a user establishes a following relationship with another user.
        /// </summary>
        Follow,

        /// <summary>
        /// Generated when a user removes a previously established following relationship.
        /// </summary>
        Unfollow,

        /// <summary>
        /// Triggered when any profile field (bio, avatar, etc.) is modified.
        /// </summary>
        ProfileUpdate,

        /// <summary>
        /// Recorded when the user changes their account password through any method.
        /// Does not include initial password set during registration.
        /// </summary>
        PasswordChange,
    }
}
