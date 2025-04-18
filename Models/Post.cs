namespace StreamierGraphQLServer.Models;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents a blog post in the system.
/// </summary>
public class Post : BaseEntity
{
    /// <summary>
    /// The title of the blog post.
    /// </summary>
    /// <remarks>
    /// Maximum length is 120 characters.
    /// Minimum length is 5 characters.
    /// </remarks>
    [StringLength(120, MinimumLength = 5)]
    public required string Title { get; set; }

    /// <summary>
    /// The main content of the blog post in Markdown format.
    /// </summary>
    /// <remarks>
    /// Maximum length is 50000 characters.
    /// Minimum length is 500 characters.
    /// </remarks>
    [Required]
    [StringLength(50000, MinimumLength = 500)]
    public required string Content { get; set; }

    /// <summary>
    /// The author of this post.
    /// </summary>
    [Required]
    public required User Author { get; set; }

    /// <summary>
    /// The collection of tags associated with this post.
    /// </summary>
    [Required]
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Whether the post is featured.
    /// </summary>
    [Required]
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Collection of comments on this post.
    /// </summary>
    [Required]
    public List<PostComment> Comments { get; set; } = [];
}
