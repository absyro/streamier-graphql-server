namespace Server.Configuration;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resend;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions();

        services.AddHttpClient<ResendClient>();

        services.Configure<ResendClientOptions>(options =>
        {
            options.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_TOKEN")!;
        });

        services.AddScoped<
            IValidator<GraphQL.Mutation.CreateSessionInput>,
            Validators.CreateSessionInputValidator
        >();

        services.AddTransient<IResend, ResendClient>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        services.AddDbContext<Contexts.AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres"))
        );

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
    }
}
