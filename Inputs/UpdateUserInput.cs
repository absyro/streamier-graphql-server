namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input model for updating user information in the system.
/// </summary>
/// <remarks>
/// This input type is used in GraphQL mutations to modify user profile data.
/// All fields except SessionId are optional to allow partial updates.
/// </remarks>
public class UpdateUserInput
{
    /// <summary>
    /// The unique session identifier for the authenticated user.
    /// </summary>
    /// <value>
    /// A non-nullable string representing the current user session.
    /// This field is required to authorize the update operation.
    /// </value>
    [Required]
    [StringLength(128)]
    public required string SessionId { get; set; }

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
