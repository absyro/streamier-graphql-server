namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

public class DeleteSessionInput
{
    [Required]
    public required string SessionId { get; set; }
}
