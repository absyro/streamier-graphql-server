namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// User notification and communication preferences.
/// </summary>
public class UserPreferences : BaseEntity
{
    /// <summary>
    /// Whether to receive marketing emails from the platform.
    /// </summary>
    [Required]
    public bool MarketingEmails { get; set; } = false;

    /// <summary>
    /// Whether to receive promotional emails from partners.
    /// </summary>
    [Required]
    public bool PartnerPromotions { get; set; } = false;

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

    /// <summary>
    /// Whether to enable quiet hours for push notifications.
    /// </summary>
    [Required]
    public bool EnableQuietHours { get; set; } = false;

    /// <summary>
    /// Start time for quiet hours (in user's local time).
    /// </summary>
    public TimeSpan? QuietHoursStart { get; set; } = new TimeSpan(22, 0, 0);

    /// <summary>
    /// End time for quiet hours (in user's local time).
    /// </summary>
    public TimeSpan? QuietHoursEnd { get; set; } = new TimeSpan(7, 0, 0);
}
