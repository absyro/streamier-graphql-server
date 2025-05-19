namespace StreamierGraphQLServer;

using StreamierGraphQLServer.Configuration;
using StreamierGraphQLServer.GraphQL.Middleware;

/// <summary>
/// The main entry point for the application. Configures and runs the web server.
/// </summary>
public static class Program
{
    /// <summary>
    /// Application entry point. Configures the web host, services, middleware pipeline, and starts the application.
    /// </summary>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
        }

        app.UseCors();
        app.UseRateLimiter();
        app.UseMiddleware<SessionHeaderMiddleware>();

        app.MapGraphQL();

        app.Run();
    }
}
