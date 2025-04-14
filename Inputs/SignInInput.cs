namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

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
