namespace Server.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[GraphQLDescription("The model for sessions.")]
public class Session : Base.BaseEntity
{
    [Required]
    [GraphQLDescription("The ID of the user associated with the session.")]
    [Column("user_id")]
    public required string UserId { get; set; }

    [Required]
    [GraphQLDescription("The date and time when the session expires.")]
    [Column("expires_at")]
    public required DateTime ExpiresAt { get; set; }
}
