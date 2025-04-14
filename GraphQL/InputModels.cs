namespace StreamierGraphQLServer.GraphQL;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models;

public class SignUpInput
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(
        30,
        MinimumLength = 3,
        ErrorMessage = "Username must be between 3 and 30 characters long."
    )]
    [RegularExpression(
        @"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username can only contain letters, numbers, and underscores."
    )]
    public required string Username { get; set; }
}

public class SignInInput
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }

    [Range(1, 365, ErrorMessage = "Expiration days must be between 1 and 365.")]
    public int ExpirationDays { get; set; } = 30;
}

public record DeleteSessionInput(
    [Required(ErrorMessage = "Session ID is required.")] string SessionId
);

public record CreateTempCodeForIdInput(
    [Required(ErrorMessage = "Purpose is required.")] TempCode.TempCodePurpose Purpose,
    [Required(ErrorMessage = "User ID is required.")] string ForId
);
