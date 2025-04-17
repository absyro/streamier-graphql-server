namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Configures a user's notification preferences and communication settings.
/// Controls how and when the user receives system notifications and alerts.
/// </summary>
/// <remarks>
/// Default values are set to provide a balanced notification experience for new users.
/// All preferences can be adjusted through the user settings interface.
/// </remarks>
public class UserPreferences : BaseEntity
{
    /// <summary>
    /// Whether push notifications are enabled globally.
    /// When disabled, all push notification types are suppressed.
    /// Default: true (enabled)
    /// </summary>
    [Required]
    public bool PushNotifications { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications when gaining new followers.
    /// Only active when <see cref="PushNotifications"/> is true.
    /// Default: true (enabled)
    /// </summary>
    [Required]
    public bool PushOnNewFollowers { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications when mentioned (@username) in content.
    /// Only active when <see cref="PushNotifications"/> is true.
    /// Default: true (enabled)
    /// </summary>
    [Required]
    public bool PushOnMentions { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for comments on the user's content.
    /// Only active when <see cref="PushNotifications"/> is true.
    /// Default: true (enabled)
    /// </summary>
    [Required]
    public bool PushOnComments { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for replies to the user's comments.
    /// Only active when <see cref="PushNotifications"/> is true.
    /// Default: true (enabled)
    /// </summary>
    [Required]
    public bool PushOnCommentReplies { get; set; } = true;

    /// <summary>
    /// Whether to receive push notifications for personalized content recommendations.
    /// Only active when <see cref="PushNotifications"/> is true.
    /// Default: true (enabled)
    /// </summary>
    [Required]
    public bool PushOnRecommendations { get; set; } = true;
}
