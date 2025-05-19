namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents a user account in the system, containing authentication details,
/// profile information, preferences, and social connections.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// The user's primary email address. This is used for authentication,
    /// communication, and account recovery. Must be a valid email format.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    /// <summary>
    /// The user's unique username for public display.
    /// </summary>
    [Required]
    [StringLength(30, MinimumLength = 3)]
    [RegularExpression(
        @"^[a-z0-9_]+$",
        ErrorMessage = "Username can only contain lowercase letters, numbers, and underscores."
    )]
    public required string Username { get; set; }

    /// <summary>
    /// The user's biographical information or description.
    /// This is displayed on the user's public profile when visible per privacy settings.
    /// </summary>
    [StringLength(500)]
    public string Bio { get; set; } = "Hi! I'm using Streamier.";

    /// <summary>
    /// A value indicating whether the user's email address
    /// has been verified through the email confirmation process.
    /// Defaults to false for new users.
    /// </summary>
    [Required]
    public bool IsEmailVerified { get; set; } = false;

    /// <summary>
    /// The bcrypt-hashed password for the user account.
    /// This field is excluded from GraphQL responses for security.
    /// </summary>
    [GraphQLIgnore]
    [Required]
    [StringLength(256)]
    public required string HashedPassword { get; set; }

    /// <summary>
    /// The two-factor authentication configuration for this user.
    /// </summary>
    [GraphQLIgnore]
    public UserTwoFactorAuthentication? TwoFactorAuthentication { get; set; }

    /// <summary>
    /// The collection of active authentication sessions
    /// associated with this user account.
    /// </summary>
    public List<UserSession> Sessions { get; set; } = [];

    /// <summary>
    /// The collection of activities and interactions
    /// performed by this user, used for activity tracking and analytics.
    /// </summary>
    public List<UserActivity> Activities { get; set; } = [];

    /// <summary>
    /// The user's privacy configuration, controlling
    /// visibility of profile elements and data sharing preferences.
    /// </summary>
    [Required]
    public required UserPrivacySettings PrivacySettings { get; set; }

    /// <summary>
    /// The user's personal preferences including
    /// notification settings, display preferences, and system behavior.
    /// </summary>
    [Required]
    public required UserPreferences Preferences { get; set; }

    /// <summary>
    /// The list of users that this user is following,
    /// representing one-directional social connections.
    /// </summary>
    public List<User> Following { get; set; } = [];

    /// <summary>
    /// The collection of streams created by this user across different handlers.
    /// </summary>
    public List<UserStreams> Streams { get; set; } = [];
}
