namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// User privacy and visibility settings with comprehensive controls.
/// </summary>
public class UserPrivacySettings : BaseEntity
{
    /// <summary>
    /// Who can view the user's profile.
    /// </summary>
    [Required]
    public AudienceLevel ProfileAudience { get; set; } = AudienceLevel.Public;

    /// <summary>
    /// Who can see the user's social connections (following/followers).
    /// </summary>
    [Required]
    public AudienceLevel SocialConnectionsAudience { get; set; } = AudienceLevel.Public;

    /// <summary>
    /// Who can see the user's activities.
    /// </summary>
    [Required]
    public AudienceLevel ActivityAudience { get; set; } = AudienceLevel.Public;

    /// <summary>
    /// Who can @mention this user.
    /// </summary>
    [Required]
    public MentionPermission WhoCanMention { get; set; } = MentionPermission.Anyone;

    /// <summary>
    /// Whether new followers require manual approval.
    /// </summary>
    [Required]
    public bool RequireFollowerApproval { get; set; } = false;

    /// <summary>
    /// Whether the user's profile appears in search results.
    /// </summary>
    [Required]
    public bool ShowInSearchResults { get; set; } = true;

    /// <summary>
    /// Defines who can access content/features.
    /// </summary>
    public enum AudienceLevel
    {
        /// <summary>Anyone, including logged out users</summary>
        Public,

        /// <summary>Only approved followers</summary>
        Followers,

        /// <summary>Only the user themselves</summary>
        Private,
    }

    /// <summary>
    /// Defines who can mention this user.
    /// </summary>
    public enum MentionPermission
    {
        /// <summary>Anyone can mention</summary>
        Anyone,

        /// <summary>Only followers can mention</summary>
        Followers,

        /// <summary>No mentions allowed</summary>
        Disabled,
    }
}
