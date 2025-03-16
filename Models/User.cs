namespace Server.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[GraphQLDescription("The model for users.")]
public class User : Base.BaseEntity
{
    [Required]
    [EmailAddress]
    [GraphQLDescription("The email address of the user.")]
    [Column("email")]
    public required string Email { get; set; }

    [Required]
    [GraphQLDescription("Whether or not the user's email has been verified.")]
    [Column("is_email_verified")]
    public required bool IsEmailVerified { get; set; }

    [GraphQLIgnore]
    [Required]
    [Column("hashed_password")]
    public required string HashedPassword { get; set; }

    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public static bool ValidatePassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
