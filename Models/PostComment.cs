namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents a comment on a blog post.
/// </summary>
public class PostComment : BaseEntity
{
    /// <summary>
    /// The author of this comment.
    /// </summary>
    [Required]
    public required User Author { get; set; }

    /// <summary>
    /// The content of the comment.
    /// </summary>
    [Required]
    [StringLength(1000)]
    public required string Content { get; set; }

    /// <summary>
    /// The parent comment if this is a reply.
    /// </summary>
    [Required]
    public PostComment? ParentComment { get; set; }

    /// <summary>
    /// Collection of replies to this comment.
    /// </summary>
    [Required]
    public List<PostComment> Replies { get; set; } = [];
}
