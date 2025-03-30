namespace StreamierServer.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Snowflake.Net;

/// <summary>
/// Represents a user account in the system with authentication capabilities.
/// </summary>
/// <remarks>
/// This entity stores sensitive user information including email and password hash,
/// following security best practices for credential storage.
/// </remarks>
public class User : Base.BaseEntity
{
    private static readonly IdWorker _idWorker = new(1, 1);

    /// <summary>
    /// The user's email address used for authentication and communication.
    /// </summary>
    /// <value>
    /// A valid email address string that is unique across the system.
    /// This field is required and validated for proper email format.
    /// Maps to the 'email' column in the database.
    /// </value>
    [Required]
    [EmailAddress]
    [Column("email")]
    public required string Email { get; set; }

    /// <summary>
    /// A value indicating whether the user's email address has been verified.
    /// </summary>
    /// <value>
    /// <c>true</c> if the email address has been verified through a confirmation process;
    /// otherwise, <c>false</c>. This field is required and maps to the 'is_email_verified'
    /// column in the database.
    /// </value>
    [Required]
    [Column("is_email_verified")]
    public required bool IsEmailVerified { get; set; }

    /// <summary>
    /// The bcrypt-hashed version of the user's password.
    /// </summary>
    /// <value>
    /// A string containing the password hash and salt in bcrypt format.
    /// This field is excluded from GraphQL responses and is required.
    /// Maps to the 'hashed_password' column in the database.
    /// </value>
    [GraphQLIgnore]
    [Required]
    [Column("hashed_password")]
    public required string HashedPassword { get; set; }

    /// <summary>
    /// Generates a unique identifier using the <see cref="Snowflake"/> algorithm.
    /// </summary>
    /// <returns>
    /// A string representation of a 64-bit unique ID that is time-ordered,
    /// ensuring chronological sorting of records.
    /// </returns>
    /// <remarks>
    /// This method uses <see cref="IdWorker"/> to generate
    /// distributed unique identifiers that are roughly time-sortable.
    /// </remarks>
    public static string GenerateId()
    {
        return _idWorker.NextId().ToString();
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
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "Password cannot be empty or whitespace.",
                nameof(password)
            );
        }

        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// Verifies a plaintext password against a stored bcrypt hash.
    /// </summary>
    /// <param name="password">The plaintext password to verify.</param>
    /// <param name="hashedPassword">The stored bcrypt hash to compare against.</param>
    /// <returns>
    /// <c>true</c> if the password matches the hash; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method uses bcrypt's constant-time comparison to prevent
    /// timing attacks during password verification.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if either parameter is <c>null</c> or empty.
    /// </exception>
    public static bool ValidatePassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "Password cannot be empty or whitespace.",
                nameof(password)
            );
        }

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            throw new ArgumentException(
                "Hashed password cannot be empty or whitespace.",
                nameof(hashedPassword)
            );
        }

        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
