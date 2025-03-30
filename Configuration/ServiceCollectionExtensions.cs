namespace Server.Configuration;

using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resend;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(configuration);

        var postgresConnectionString =
            configuration.GetConnectionString("Postgres")
            ?? throw new Exceptions.ConfigurationException("Postgres connection string is missing");

        var resendApiToken =
            configuration["Resend:ApiToken"]
            ?? throw new Exceptions.ConfigurationException("Resend API Token is missing");

        services
            .AddOptions()
            .AddHttpClient<ResendClient>()
            .Services.Configure<ResendClientOptions>(options => options.ApiToken = resendApiToken);

        services.AddValidators();

        services.AddCorsPolicy();

        services.AddDatabaseContext(postgresConnectionString);

        services.AddGraphQlServer();

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
        string connectionString
    )
    {
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
