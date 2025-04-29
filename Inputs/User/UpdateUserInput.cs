namespace StreamierGraphQLServer.Inputs.User;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input model for updating user information in the system.
/// </summary>
/// <remarks>
/// This input type is used in GraphQL mutations to modify user profile data.
/// </remarks>
public class UpdateUserInput
{
    /// <summary>
    /// The biographical text for the user's profile.
    /// </summary>
    /// <value>
    /// A nullable string containing personal description or profile information,
    /// with a maximum length of 500 characters.
    /// </value>
    [StringLength(500)]
    public string? Bio { get; set; }
}
