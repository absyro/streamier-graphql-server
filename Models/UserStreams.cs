namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents a user's streams
/// </summary>
[Table("user_streams")]
public class UserStreams : BaseEntity
{
    /// <summary>
    /// The ID of the handler that the stream belongs to
    /// </summary>
    [Required]
    [StringLength(8)]
    public required string HandlerId { get; set; }
}
