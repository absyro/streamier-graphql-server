namespace StreamierGraphQLServer.Models.Users;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// Represents the two-factor authentication configuration for a user.
/// </summary>
[Table("user_two_factor_authentications")]
public class UserTwoFactorAuthentication : BaseEntity
{
    /// <summary>
    /// The secret key used for generating two-factor authentication codes.
    /// </summary>
    /// <remarks>
    /// This field should be encrypted at rest for additional security.
    /// </remarks>
    [GraphQLIgnore]
    [Required]
    [StringLength(128)]
    public required string Secret { get; set; }

    /// <summary>
    /// The collection of recovery codes for two-factor authentication.
    /// These are one-time use codes that can be used when the primary 2FA method is unavailable.
    /// </summary>
    [GraphQLIgnore]
    [Required]
    public List<string> RecoveryCodes { get; set; } = [];
}
