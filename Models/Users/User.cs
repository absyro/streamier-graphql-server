namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a user account in the system with authentication capabilities and profile settings.
/// </summary>
public class User : Base.BaseEntity
{
    /// <summary>
    /// The user's email address used for authentication and communication.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    /// <summary>
    /// A brief description about the user.
    /// </summary>
    [StringLength(500)]
    public string? Bio { get; set; }

    /// <summary>
    /// A value indicating whether the user's email address has been verified.
    /// </summary>
    [Required]
    public bool IsEmailVerified { get; set; } = false;

    /// <summary>
    /// The bcrypt-hashed version of the user's password.
    /// </summary>
    [GraphQLIgnore]
    [Required]
    [StringLength(256)]
    public required string HashedPassword { get; set; }

    /// <summary>
    /// Collection of authentication sessions associated with the user.
    /// </summary>
    public List<UserSession> Sessions { get; set; } = [];

    /// <summary>
    /// Collection of user activities and interactions.
    /// </summary>
    public List<UserActivity> Activities { get; set; } = [];

    /// <summary>
    /// User's privacy and visibility settings.
    /// </summary>
    [Required]
    public required UserPrivacySettings PrivacySettings { get; set; }

    /// <summary>
    /// User's notification and communication preferences.
    /// </summary>
    [Required]
    public required UserPreferences Preferences { get; set; }

    /// <summary>
    /// List of user IDs that this user follows.
    /// </summary>
    public List<string> Following { get; set; } = [];
}
