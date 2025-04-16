namespace StreamierGraphQLServer.Inputs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the authentication credentials and session parameters for user login.
/// </summary>
/// <remarks>
/// This input type is used in GraphQL mutations to authenticate users and establish sessions.
/// All fields are required to ensure secure authentication and explicit session management.
/// </remarks>
public class SignInInput
{
    /// <summary>
    /// The email address associated with the user account.
    /// </summary>
    /// <value>
    /// A valid email address string that matches the account's registered email.
    /// Must conform to standard email format (RFC 5322).
    /// </value>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// The secret password for account verification.
    /// </summary>
    /// <value>
    /// The account password that will be verified against the stored credentials.
    /// For security, the raw password should never be stored or logged.
    /// </value>
    [Required]
    public required string Password { get; set; }

    /// <summary>
    /// The termination timestamp for the authentication session.
    /// </summary>
    /// <value>
    /// A UTC DateTime value specifying when the session should automatically expire.
    /// Must be in the future relative to the current server time.
    /// </value>
    [Required]
    public required DateTime ExpirationDate { get; set; }
}
