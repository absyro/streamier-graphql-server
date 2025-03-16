namespace Server.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

[GraphQLDescription("The model for temp codes.")]
public class TempCode : Base.BaseEntity
{
    [Required]
    [GraphQLDescription("The purpose of the temp code.")]
    [Column("purpose")]
    public required TempCodePurpose Purpose { get; set; }

    [Required]
    [GraphQLDescription("The ID of the entity the temp code is for.")]
    [Column("for_id")]
    public required string ForId { get; set; }

    [GraphQLIgnore]
    [Required]
    [Column("hashed_code")]
    public required string HashedCode { get; set; }

    [GraphQLIgnore]
    [Required]
    [Column("code_salt")]
    public required string CodeSalt { get; set; }

    [Required]
    [GraphQLDescription("The date and time the temp code expires.")]
    [Column("expires_at")]
    public required DateTime ExpiresAt { get; set; }

    [GraphQLDescription("The temp code's purposes.")]
    public enum TempCodePurpose
    {
        ChangePassword,
        ChangeEmail,
        EmailVerification,
        DeleteAccount,
    }

    public static string GenerateCodeSalt()
    {
        var buffer = new byte[16];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(buffer);

        return Convert.ToBase64String(buffer);
    }

    public static string HashCode(string code, string salt)
    {
        var saltedCode = code + salt;

        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltedCode));

        return Convert.ToBase64String(hashedBytes);
    }

    public static bool ValidateCode(string code, string hashedCode, string salt)
    {
        var hashedInput = HashCode(code, salt);

        return hashedInput == hashedCode;
    }
}
