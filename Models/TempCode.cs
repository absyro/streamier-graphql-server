namespace StreamierServer.Models;

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using RandomString4Net;

/// <summary>
/// Represents a temporary security code used for sensitive operations in the system.
/// </summary>
/// <remarks>
/// This entity stores hashed versions of temporary codes along with their salts,
/// following security best practices. The actual code values are never stored directly.
/// </remarks>
public class TempCode : Base.BaseEntity
{
    /// <summary>
    /// The intended use case for this temporary code.
    /// </summary>
    /// <value>
    /// A <see cref="TempCodePurpose"/> enum value indicating the code's purpose.
    /// This field is required.
    /// </value>
    [Required]
    public required TempCodePurpose Purpose { get; set; }

    /// <summary>
    /// The identifier of the entity this code is associated with.
    /// </summary>
    /// <value>
    /// A string representing the ID of the user or entity this code relates to.
    /// This field is required.
    /// </value>
    [Required]
    public required string ForId { get; set; }

    /// <summary>
    /// The SHA256-hashed version of the temporary code.
    /// </summary>
    /// <value>
    /// A base64-encoded string representing the hashed code.
    /// This field is excluded from GraphQL responses and is required.
    /// </value>
    [GraphQLIgnore]
    [Required]
    public required string HashedCode { get; set; }

    /// <summary>
    /// The cryptographically random salt used for hashing the code.
    /// </summary>
    /// <value>
    /// A base64-encoded string representing the 16-byte random salt.
    /// This field is excluded from GraphQL responses and is required.
    /// </value>
    [GraphQLIgnore]
    [Required]
    public required string CodeSalt { get; set; }

    /// <summary>
    /// The UTC date and time when this code becomes invalid.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> in UTC format indicating the code's expiration.
    /// After this time, the code cannot be used for verification.
    /// This field is required.
    /// </value>
    [Required]
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Specifies the possible purposes for temporary codes in the system.
    /// </summary>
    public enum TempCodePurpose
    {
        /// <summary>
        /// Code used for password reset or change operations.
        /// </summary>
        ChangePassword,

        /// <summary>
        /// Code used for email address change verification.
        /// </summary>
        ChangeEmail,

        /// <summary>
        /// Code used for initial email address verification.
        /// </summary>
        EmailVerification,

        /// <summary>
        /// Code used for account deletion confirmation.
        /// </summary>
        DeleteAccount,
    }

    /// <summary>
    /// Generates a random alphanumeric code suitable for temporary verification purposes.
    /// </summary>
    /// <returns>A lowercase alphanumeric string of 16 characters.</returns>
    public static string GenerateCode()
    {
        return RandomString.GetString(Types.ALPHANUMERIC_LOWERCASE, 16);
    }

    /// <summary>
    /// Generates a cryptographically secure random salt for code hashing.
    /// </summary>
    /// <returns>
    /// A base64-encoded string representing 16 bytes of cryptographically random data.
    /// </returns>
    /// <remarks>
    /// This method uses <see cref="RandomNumberGenerator"/> to ensure cryptographic strength.
    /// </remarks>
    public static string GenerateCodeSalt()
    {
        var buffer = new byte[16];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(buffer);

        return Convert.ToBase64String(buffer);
    }

    /// <summary>
    /// Computes the SHA256 hash of a code combined with a salt.
    /// </summary>
    /// <param name="code">The plaintext code to hash.</param>
    /// <param name="salt">The salt value to combine with the code.</param>
    /// <returns>
    /// A base64-encoded string representing the SHA256 hash of the salted code.
    /// </returns>
    /// <remarks>
    /// The hashing process follows the pattern: SHA256(code + salt).
    /// </remarks>
    public static string HashCode(string code, string salt)
    {
        var saltedCode = code + salt;

        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltedCode));

        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verifies whether a provided code matches the stored hashed code.
    /// </summary>
    /// <param name="code">The plaintext code to verify.</param>
    /// <param name="hashedCode">The stored hashed code to compare against.</param>
    /// <param name="salt">The salt used in the original hashing operation.</param>
    /// <returns>
    /// <c>true</c> if the provided code produces the same hash when combined with the salt;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method performs a constant-time comparison to prevent timing attacks.
    /// </remarks>
    public static bool ValidateCode(string code, string hashedCode, string salt)
    {
        var hashedInput = HashCode(code, salt);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hashedInput),
            Encoding.UTF8.GetBytes(hashedCode)
        );
    }
}
