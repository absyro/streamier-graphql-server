namespace StreamierGraphQLServer.GraphQL.Mutations;

using Microsoft.EntityFrameworkCore;
using OtpNet;
using RandomString4Net;
using StreamierGraphQLServer.Contexts;
using StreamierGraphQLServer.GraphQL.Context;
using StreamierGraphQLServer.Inputs.Auth;
using StreamierGraphQLServer.Models;

/// <summary>
/// Contains mutations related to two-factor authentication management.
/// </summary>
[ExtendObjectType("Mutation")]
public class TwoFactorAuthMutations
{
    /// <summary>
    /// Enables two-factor authentication for the current user.
    /// </summary>
    public async Task<EnableTwoFactorAuthenticationResult> EnableTwoFactorAuthentication(
        [Service] AppDbContext dbContext,
        [Service] GraphQLContext graphQLContext
    )
    {
        var sessionId = graphQLContext.GetSessionId();

        if (string.IsNullOrEmpty(sessionId))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Session ID is required")
                    .SetCode("SESSION_REQUIRED")
                    .Build()
            );
        }

        var user = await dbContext
            .Users.Include(u => u.TwoFactorAuthentication)
            .FirstOrDefaultAsync(u => u.Sessions.Any(s => s.Id == sessionId));

        if (user == null)
        {
            throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );
        }

        if (user.TwoFactorAuthentication != null)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Two-factor authentication is already enabled")
                    .SetCode("2FA_ALREADY_ENABLED")
                    .Build()
            );
        }

        var secret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
        var recoveryCodes = GenerateRecoveryCodes(count: 8);

        user.TwoFactorAuthentication = new UserTwoFactorAuthentication
        {
            Id = user.Id,
            Secret = secret,
            RecoveryCodes = new List<string>(recoveryCodes),
        };

        await dbContext.SaveChangesAsync();

        return new EnableTwoFactorAuthenticationResult
        {
            SecretKey = secret,
            RecoveryCodes = recoveryCodes,
        };
    }

    /// <summary>
    /// Disables two-factor authentication for the current user.
    /// </summary>
    public async Task<bool> DisableTwoFactorAuthentication(
        [Service] AppDbContext dbContext,
        [Service] GraphQLContext graphQLContext,
        DisableTwoFactorAuthenticationInput input
    )
    {
        var sessionId = graphQLContext.GetSessionId();

        if (string.IsNullOrEmpty(sessionId))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Session ID is required")
                    .SetCode("SESSION_REQUIRED")
                    .Build()
            );
        }

        var user = await dbContext
            .Users.Include(u => u.TwoFactorAuthentication)
            .FirstOrDefaultAsync(u => u.Sessions.Any(s => s.Id == sessionId));

        if (user == null)
        {
            throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );
        }

        if (user.TwoFactorAuthentication == null)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Two-factor authentication is not enabled")
                    .SetCode("2FA_NOT_ENABLED")
                    .Build()
            );
        }

        if (!BCrypt.Net.BCrypt.Verify(input.Password, user.HashedPassword))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Invalid password")
                    .SetCode("INVALID_PASSWORD")
                    .Build()
            );
        }

        dbContext.Remove(user.TwoFactorAuthentication);

        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Generates new recovery codes for two-factor authentication.
    /// </summary>
    public async Task<List<string>> GenerateNewRecoveryCodes(
        [Service] AppDbContext dbContext,
        [Service] GraphQLContext graphQLContext
    )
    {
        var sessionId = graphQLContext.GetSessionId();

        if (string.IsNullOrEmpty(sessionId))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Session ID is required")
                    .SetCode("SESSION_REQUIRED")
                    .Build()
            );
        }

        var user = await dbContext
            .Users.Include(u => u.TwoFactorAuthentication)
            .FirstOrDefaultAsync(u => u.Sessions.Any(s => s.Id == sessionId));

        if (user == null)
        {
            throw new GraphQLException(
                ErrorBuilder.New().SetMessage("User not found").SetCode("USER_NOT_FOUND").Build()
            );
        }

        if (user.TwoFactorAuthentication == null)
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Two-factor authentication is not enabled")
                    .SetCode("2FA_NOT_ENABLED")
                    .Build()
            );
        }

        var recoveryCodes = GenerateRecoveryCodes(count: 8);

        user.TwoFactorAuthentication.RecoveryCodes = new List<string>(recoveryCodes);

        await dbContext.SaveChangesAsync();

        return recoveryCodes;
    }

    /// <summary>
    /// Result type for enabling two-factor authentication.
    /// </summary>
    public class EnableTwoFactorAuthenticationResult
    {
        /// <summary>
        /// The secret key to use with authenticator apps.
        /// </summary>
        public required string SecretKey { get; set; }

        /// <summary>
        /// Recovery codes that can be used as backup if the authenticator is lost.
        /// </summary>
        public required List<string> RecoveryCodes { get; set; }
    }

    private static List<string> GenerateRecoveryCodes(int count)
    {
        var recoveryCodes = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            recoveryCodes.Add(RandomString.GetString(Types.ALPHANUMERIC_UPPERCASE, 8));
        }

        return recoveryCodes;
    }
}
