namespace StreamierGraphQLServer.Models.Base;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Abstract base class that provides common properties for all entities in the system.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    [GraphQLNonNullType]
    [GraphQLType(typeof(IdType))]
    [Key]
    [StringLength(128)]
    public required string Id { get; set; }

    /// <summary>
    /// The UTC date and time when the entity was initially created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC date and time when the entity was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
