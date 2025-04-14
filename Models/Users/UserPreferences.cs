namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// User notification and communication preferences.
/// </summary>
public class UserPreferences : BaseEntity
{
    /// <summary>
    /// Whether to allow push notifications.
    /// </summary>
    [Required]
    public bool PushNotifications { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for new followers.
    /// </summary>
    [Required]
    public bool PushOnNewFollowers { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for mentions.
    /// </summary>
    [Required]
    public bool PushOnMentions { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for comments on your content.
    /// </summary>
    [Required]
    public bool PushOnComments { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for replies to your comments.
    /// </summary>
    [Required]
    public bool PushOnCommentReplies { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for content recommendations.
    /// </summary>
    [Required]
    public bool PushOnRecommendations { get; set; } = false;
}
