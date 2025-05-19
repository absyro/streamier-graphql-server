namespace StreamierGraphQLServer.Inputs.User;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input model for updating user information in the system.
/// </summary>
public class UpdateUserInput
{
    /// <summary>
    /// The biographical text for the user's profile.
    /// </summary>
    [StringLength(500)]
    public string? Bio { get; set; }
}
