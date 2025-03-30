namespace Server;

using Server.Configuration;

/// <summary>
/// The main entry point for the application.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point for the application.
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

        app.MapGraphQL();
        app.MapHealthChecks("/health");

        app.Run();
    }
}
