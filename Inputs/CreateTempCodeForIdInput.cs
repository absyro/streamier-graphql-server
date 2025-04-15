namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models;

/// <summary>
/// Represents the input required to create a temporary code for an entity.
/// </summary>
public class CreateTempCodeInput
{
    /// <summary>
    /// The purpose for which the temporary code will be used.
    /// </summary>
    [Required]
    public required TempCode.TempCodePurpose Purpose { get; set; }

    /// <summary>
    /// The identifier of the entity for which the temporary code is being created.
    /// </summary>
    [Required]
    public required string EntityId { get; set; }
}
