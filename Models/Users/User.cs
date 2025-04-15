namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RandomString4Net;
using StreamierGraphQLServer.Contexts;

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

    /// <summary>
    /// Generates a unique identifier for the user.
    /// </summary>
    /// <param name="dbContext">The database context to check for uniqueness against.</param>
    /// <returns>
    /// A string representing a unique identifier for the user.
    /// </returns>
    /// <remarks>
    /// Uses cryptographically secure random generation to create 8-character
    /// lowercase alphanumeric IDs, verifying uniqueness against the database.
    /// </remarks>
    public static async Task<string> GenerateIdAsync(AppDbContext dbContext)
    {
        string id;

        do
        {
            id = RandomString.GetString(Types.ALPHANUMERIC_LOWERCASE, 8);
        } while (await dbContext.Users.AnyAsync(u => u.Id == id));

        return id;
    }

    /// <summary>
    /// Securely hashes a password using bcrypt with a work factor of 12.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>
    /// A string containing the bcrypt hash of the password,
    /// including the salt and work factor.
    /// </returns>
    /// <remarks>
    /// The work factor of 12 provides a good balance between security
    /// and performance (approximately 250-300ms hash time on modern hardware).
    /// </remarks>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// Verifies a plaintext password against a stored bcrypt hash.
    /// </summary>
    /// <param name="password">The plaintext password to verify.</param>
    /// <param name="hashedPassword">The stored bcrypt hash to compare against.</param>
    /// <returns>
    /// true if the password matches the hash; otherwise, false.
    /// </returns>
    /// <remarks>
    /// This method uses bcrypt's constant-time comparison to prevent
    /// timing attacks during password verification.
    /// </remarks>
    public static bool ValidatePassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
