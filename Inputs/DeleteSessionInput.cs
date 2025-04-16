namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input required to terminate and invalidate a user session.
/// </summary>
/// <remarks>
/// This input type is used in GraphQL mutations to securely end authentication sessions.
/// Typically used for logout functionality or session revocation.
/// </remarks>
public class DeleteSessionInput
{
    /// <summary>
    /// The unique identifier of the session to be terminated.
    /// </summary>
    /// <value>
    /// A non-nullable string representing the session's unique key.
    /// </value>
    [Required]
    public required string SessionId { get; set; }
}
