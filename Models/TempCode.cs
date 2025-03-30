namespace Server.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using RandomString4Net;

/// <summary>
/// Represents a temporary code in the system.
/// </summary>
public class TempCode : Base.BaseEntity
{
    /// <summary>
    /// The purpose of the temp code.
    /// </summary>
    [Required]
    [Column("purpose")]
    public required TempCodePurpose Purpose { get; set; }

    /// <summary>
    /// The ID of the entity associated with the temp code.
    /// </summary>
    [Required]
    [Column("for_id")]
    public required string ForId { get; set; }

    /// <summary>
    /// The hashed temp code.
    /// </summary>
    [GraphQLIgnore]
    [Required]
    [Column("hashed_code")]
    public required string HashedCode { get; set; }

    /// <summary>
    /// The salt used to hash the temp code.
    /// </summary>
    [GraphQLIgnore]
    [Required]
    [Column("code_salt")]
    public required string CodeSalt { get; set; }

    /// <summary>
    /// The expiration date of the temp code.
    /// </summary>
    [Required]
    [Column("expires_at")]
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// The purpose of the temp code.
    /// </summary>
    public enum TempCodePurpose
    {
        /// <summary>
        /// This temp code is for changing a user's password.
        /// </summary>
        ChangePassword,

        /// <summary>
        /// This temp code is for changing a user's email address.
        /// </summary>
        ChangeEmail,

        /// <summary>
        ///  This temp code is for verifying a user's email address.
        /// </summary>
        EmailVerification,

        /// <summary>
        /// This temp code is for deleting a user's account.
        /// </summary>
        DeleteAccount,
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string GenerateCode(int length = 16)
    {
        return RandomString.GetString(Types.ALPHANUMERIC_LOWERCASE, length);
    }

    /// <summary>
    /// Generates a cryptographically secure random salt for hashing the temp code.
    /// </summary>
    /// <returns>A base64-encoded random salt.</returns>
    public static string GenerateCodeSalt()
    {
        var buffer = new byte[16];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(buffer);

        return Convert.ToBase64String(buffer);
    }

    /// <summary>
    /// Hashes a temp code using SHA256.
    /// </summary>
    /// <param name="code">The temp code to hash.</param>
    /// <param name="salt">The salt to use for hashing.</param>
    /// <returns>The hashed temp code.</returns>
    public static string HashCode(string code, string salt)
    {
        var saltedCode = code + salt;

        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltedCode));

        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Validates a temp code against a hashed temp code using SHA256.
    /// </summary>
    /// <param name="code">The temp code to validate.</param>
    /// <param name="hashedCode">The hashed temp code to validate against.</param>
    /// <param name="salt">The salt used to hash the temp code.</param>
    /// <returns>True if the temp code is valid, false otherwise.</returns>
    public static bool ValidateCode(string code, string hashedCode, string salt)
    {
        var hashedInput = HashCode(code, salt);

        return hashedInput == hashedCode;
    }
}
