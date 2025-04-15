namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input required to delete a user session.
/// </summary>
public class DeleteSessionInput
{
    /// <summary>
    /// The unique identifier of the session to be deleted.
    /// </summary>
    [Required]
    public required string SessionId { get; set; }
}
