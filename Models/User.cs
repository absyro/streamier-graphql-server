namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RandomString4Net;
using StreamierGraphQLServer.Contexts;

/// <summary>
/// Represents a user account in the system with authentication capabilities.
/// </summary>
/// <remarks>
/// This entity stores sensitive user information including email and password hash,
/// following security best practices for credential storage.
/// </remarks>
public class User : Base.BaseEntity
{
    /// <summary>
    /// The user's email address used for authentication and communication.
    /// </summary>
    /// <value>
    /// A valid email address string that is unique across the system.
    /// This field is required and validated for proper email format.
    /// </value>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    /// <summary>
    /// A value indicating whether the user's email address has been verified.
    /// </summary>
    /// <value>
    /// true if the email address has been verified through a confirmation process;
    /// otherwise, false. This field is required.
    /// </value>
    [Required]
    public required bool IsEmailVerified { get; set; }

    /// <summary>
    /// The bcrypt-hashed version of the user's password.
    /// </summary>
    /// <value>
    /// A string containing the password hash and salt in bcrypt format.
    /// This field is excluded from GraphQL responses and is required.
    /// </value>
    [GraphQLIgnore]
    [Required]
    [StringLength(256)]
    public required string HashedPassword { get; set; }

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
