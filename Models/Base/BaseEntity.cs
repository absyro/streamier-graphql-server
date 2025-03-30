namespace StreamierServer.Models.Base;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    /// This value is required and maps to the 'id' column in the database.
    /// </value>
    [Key]
    [Column("id")]
    public required string Id { get; set; }

    /// <summary>
    /// The UTC date and time when the entity was initially created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> in UTC format representing the creation timestamp.
    /// This value is automatically set to the current UTC time when the entity is created
    /// and maps to the 'created_at' column in the database.
    /// </value>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC date and time when the entity was last modified.
    /// </summary>
    /// <value>
    /// A nullable <see cref="DateTime"/> in UTC format representing the last update timestamp.
    /// This value is null if the entity has never been updated after creation
    /// and maps to the 'updated_at' column in the database.
    /// </value>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
