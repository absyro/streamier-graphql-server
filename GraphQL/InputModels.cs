namespace StreamierGraphQLServer.GraphQL;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models;

public class SignUpInput
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    [RegularExpression(
        @"^[a-z0-9_]+$",
        ErrorMessage = "Username can only contain lowercase letters, numbers, and underscores."
    )]
    public required string Username { get; set; }
}

public class SignInInput
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Range(1, 365)]
    public int ExpirationDays { get; set; } = 30;
}

public record DeleteSessionInput([Required] string SessionId);

public record CreateTempCodeForIdInput(
    [Required] TempCode.TempCodePurpose Purpose,
    [Required] string ForId
);
