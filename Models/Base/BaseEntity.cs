namespace StreamierGraphQLServer.Models.Base;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Abstract base class that provides common properties for all entities in the system.
/// </summary>
/// <remarks>
/// This class serves as the foundation for all domain entities, ensuring consistent
/// implementation of common properties like identifiers and audit fields.
/// </remarks>
public abstract class BaseEntity
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    /// <value>
    /// A string that represents the primary key of the entity.
    /// This field is required.
    /// </value>
    [GraphQLType(typeof(IdType))]
    [Key]
    [StringLength(128)]
    public required string Id { get; set; }

    /// <summary>
    /// The UTC date and time when the entity was initially created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> in UTC format representing the creation timestamp.
    /// This field is automatically set to the current UTC time when the entity is created.
    /// </value>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC date and time when the entity was last modified.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> in UTC format representing the last update timestamp.
    /// This field is automatically set to the current UTC time when the entity is created.
    /// </value>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
