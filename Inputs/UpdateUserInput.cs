namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input required to update a user.
/// </summary>
public class UpdateUserInput
{
    /// <summary>
    /// The user's session ID.
    /// </summary>
    [Required]
    public required string SessionId { get; set; }

    /// <summary>
    /// A brief description about the user.
    /// </summary>
    [StringLength(500)]
    public string? Bio { get; set; }
}
