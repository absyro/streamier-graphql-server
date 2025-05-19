namespace StreamierGraphQLServer.Configuration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Resend;
using StreamierGraphQLServer.Exceptions;

/// <summary>
/// Provides extension methods for configuring and adding services to the <see cref="IServiceCollection"/>.
/// This class centralizes all service registrations for the application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services with the dependency injection container.
    /// This is the main entry point for service configuration.
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddConfiguration(configuration);
        services.AddHttpServices();
        services.AddRateLimiting();
        services.AddCorsPolicy();
        services.AddDatabaseContext(configuration);
        services.AddHttpContextAccessor();
        services.AddGraphQLContext();
        services.AddGraphQlServer();

        return services;
    }

    /// <summary>
    /// Configures application settings from the configuration.
    /// </summary>
    private static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var resendApiToken =
            configuration["Resend:ApiToken"]
            ?? throw new ConfigurationException("Resend API Token is missing");

        services.Configure<ResendClientOptions>(options =>
        {
            options.ApiToken = resendApiToken;
        });

        return services;
    }

    /// <summary>
    /// Registers HTTP client services used by the application.
    /// </summary>
    private static IServiceCollection AddHttpServices(this IServiceCollection services)
    {
        services.AddHttpClient<ResendClient>();

        return services;
    }

    /// <summary>
    /// Configures rate limiting for the application.
    /// Uses a fixed window rate limiter with IP-based partitioning.
    /// </summary>
    private static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "anonymous",
                        factory: _ =>
                        {
                            return new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 15,
                                Window = TimeSpan.FromSeconds(10),
                            };
                        }
                    );
                }
            );

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (!context.HttpContext.Response.HasStarted)
                {
                    await context.HttpContext.Response.WriteAsync(
                        "Too Many Requests",
                        cancellationToken
                    );
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Configures CORS (Cross-Origin Resource Sharing) policy for the application.
    /// Uses a permissive policy that allows requests from any origin, with any header and method.
    /// </summary>
    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }

    /// <summary>
    /// Registers the application's database context with the dependency injection container.
    /// Configures the context to use PostgreSQL with the connection string from configuration.
    /// </summary>
    private static IServiceCollection AddDatabaseContext(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("Postgres")
            ?? throw new ConfigurationException("Postgres connection string is missing");

        services.AddDbContext<Contexts.AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        return services;
    }

    /// <summary>
    /// Registers the GraphQL context used for session management.
    /// </summary>
    private static IServiceCollection AddGraphQLContext(this IServiceCollection services)
    {
        services.AddScoped<GraphQL.Context.GraphQLContext>();

        return services;
    }

    /// <summary>
    /// Configures the GraphQL server with all necessary types, conventions, and features.
    /// Includes support for queries, mutations, filtering, projections, and paging.
    /// </summary>
    private static IServiceCollection AddGraphQlServer(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .RegisterDbContextFactory<Contexts.AppDbContext>()
            .AddQueryConventions()
            .AddMutationConventions(applyToAllMutations: true)
            .AddQueryType<GraphQL.Queries.Query>()
            .AddTypeExtension<GraphQL.Queries.UserQueries>()
            .AddMutationType<GraphQL.Mutations.Mutation>()
            .AddTypeExtension<GraphQL.Mutations.AuthMutations>()
            .AddTypeExtension<GraphQL.Mutations.UserMutations>()
            .AddTypeExtension<GraphQL.Mutations.TwoFactorAuthMutations>()
            .ModifyPagingOptions(options =>
            {
                options.IncludeTotalCount = true;
            })
            .AddFiltering()
            .AddProjections();

        return services;
    }
}
