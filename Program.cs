namespace StreamierServer;

using StreamierServer.Configuration;

// TODO: User ID Should be unique and 16 chars
// TODO: Use varchar

/// <summary>
/// The main entry point for the application. Configures and runs the web server.
/// </summary>
public static class Program
{
    /// <summary>
    /// Application entry point. Configures the web host, services, middleware pipeline, and starts the application.
    /// </summary>
    /// <param name="args">Command line arguments passed during application startup.</param>
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

        app.MapGraphQL();
        app.MapHealthChecks("/health");

        app.Run();
    }
}
