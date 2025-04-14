namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;
using StreamierGraphQLServer.Models;

public class CreateTempCodeForIdInput
{
    [Required]
    public required TempCode.TempCodePurpose Purpose { get; set; }

    [Required]
    public required string ForId { get; set; }
}
