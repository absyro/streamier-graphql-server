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

    [Required]
    public required DateTime ExpirationDate { get; set; }
}

public class DeleteSessionInput
{
    [Required]
    public required string SessionId { get; set; }
}

public class CreateTempCodeForIdInput
{
    [Required]
    public required TempCode.TempCodePurpose Purpose { get; set; }

    [Required]
    public required string ForId { get; set; }
}
