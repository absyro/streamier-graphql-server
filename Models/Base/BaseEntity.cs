namespace Server.Models.Base;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class BaseEntity
{
    [Key]
    [GraphQLDescription("The unique identifier of the entity.")]
    [Column("id")]
    public required string Id { get; set; }

    [GraphQLDescription("The date and time when the entity was created.")]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [GraphQLDescription("The date and time when the entity was last updated.")]
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
