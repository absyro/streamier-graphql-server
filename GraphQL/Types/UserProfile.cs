namespace StreamierGraphQLServer.GraphQL.Types;

using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents a user profile with basic information.
/// </summary>
public class UserProfile : BaseEntity
{
    /// <summary>
    /// The biography or description of the user.
    /// </summary>
    public required string Bio { get; set; }
}
