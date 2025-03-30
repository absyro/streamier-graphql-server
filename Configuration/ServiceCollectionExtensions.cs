namespace StreamierServer.Configuration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resend;

/// <summary>
/// Represents the extension methods for the service collection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    private const int RateLimitPermits = 15;

    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Adds the application services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddConfiguration(configuration);
        services.AddHttpServices(configuration);
        services.AddHealthChecks();
        services.AddRateLimiting();
        services.AddValidators();
        services.AddCorsPolicy();
        services.AddDatabaseContext(configuration);
        services.AddGraphQlServer();

        return services;
    }

    private static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var resendApiToken =
            configuration["Resend:ApiToken"]
            ?? throw new Exceptions.ConfigurationException("Resend API Token is missing");

        services.Configure<ResendClientOptions>(options => options.ApiToken = resendApiToken);

        return services;
    }

    private static IServiceCollection AddHttpServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHttpClient<ResendClient>();

        return services;
    }

    private static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "anonymous",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = RateLimitPermits,
                            Window = RateLimitWindow,
                        }
                    )
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

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<
            IValidator<GraphQL.Mutation.CreateSessionInput>,
            Validators.CreateSessionInputValidator
        >();

        services.AddTransient<IResend, ResendClient>();

        return services;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            );
        });

        return services;
    }

    private static IServiceCollection AddDatabaseContext(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("Postgres")
            ?? throw new Exceptions.ConfigurationException("Postgres connection string is missing");

        services.AddDbContext<Contexts.AppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        return services;
    }

    private static IServiceCollection AddGraphQlServer(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .RegisterDbContextFactory<Contexts.AppDbContext>()
            .AddQueryConventions()
            .AddMutationConventions(applyToAllMutations: true)
            .AddQueryType<GraphQL.Query>()
            .AddMutationType<GraphQL.Mutation>()
            .ModifyPagingOptions(options => options.IncludeTotalCount = true)
            .AddFiltering()
            .AddProjections();

        return services;
    }
}
