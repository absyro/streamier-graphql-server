namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

public class SignUpInput
{
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 8)]
    public required string Password { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    [RegularExpression(
        @"^[a-z0-9_]+$",
        ErrorMessage = "Username can only contain lowercase letters, numbers, and underscores."
    )]
    public required string Username { get; set; }
}
