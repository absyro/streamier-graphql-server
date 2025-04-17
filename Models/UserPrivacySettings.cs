namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Controls the visibility of user data and interactions across the platform.
/// Provides granular privacy controls for different aspects of the user profile.
/// </summary>
/// <remarks>
/// Default values are set to public visibility to maximize discoverability,
/// while allowing users to restrict access as needed.
/// </remarks>
public class UserPrivacySettings : BaseEntity
{
    /// <summary>
    /// Who can view the user's profile information.
    /// Controls access to bio, profile picture, and basic account details.
    /// Default: <see cref="AudienceLevel.Public"/>
    /// </summary>
    [Required]
    public AudienceLevel ProfileAudience { get; set; } = AudienceLevel.Public;

    /// <summary>
    /// Who can see the user's social graph (following/followers lists).
    /// Default: <see cref="AudienceLevel.Public"/>
    /// </summary>
    [Required]
    public AudienceLevel SocialConnectionsAudience { get; set; } = AudienceLevel.Public;

    /// <summary>
    /// Who can view the user's activity feed and recent actions.
    /// Default: <see cref="AudienceLevel.Public"/>
    /// </summary>
    [Required]
    public AudienceLevel ActivityAudience { get; set; } = AudienceLevel.Public;

    /// <summary>
    /// Who is permitted to @mention this user in comments and content.
    /// Default: <see cref="MentionPermission.Anyone"/>
    /// </summary>
    [Required]
    public MentionPermission WhoCanMention { get; set; } = MentionPermission.Anyone;

    /// <summary>
    /// Whether new follower requests require explicit approval.
    /// When enabled, follows become pending until approved by the user.
    /// Default: false (follows are automatically accepted)
    /// </summary>
    [Required]
    public bool RequireFollowerApproval { get; set; } = false;

    /// <summary>
    /// Whether the user's profile appears in platform search results.
    /// When disabled, the profile can only be accessed via direct link.
    /// Default: true (profile is searchable)
    /// </summary>
    [Required]
    public bool ShowInSearchResults { get; set; } = true;

    /// <summary>
    /// Defines the audience levels for content visibility and access control.
    /// </summary>
    public enum AudienceLevel
    {
        /// <summary>
        /// Visible to anyone, including unauthenticated users
        /// </summary>
        Public,

        /// <summary>
        /// Visible only to approved followers
        /// </summary>
        Followers,

        /// <summary>
        /// Visible only to the account owner
        /// </summary>
        Private,
    }

    /// <summary>
    /// Defines permissions for who can mention this user in content.
    /// </summary>
    public enum MentionPermission
    {
        /// <summary>
        /// Any user can mention this account
        /// </summary>
        Anyone,

        /// <summary>
        /// Only approved followers can mention this account
        /// </summary>
        Followers,

        /// <summary>
        /// Mentions are completely disabled for this account
        /// </summary>
        Disabled,
    }
}
