namespace Server;

using Server.Configuration;

public static class Program
{
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
