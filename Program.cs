namespace Server;

using System.Threading.RateLimiting;
using Server.Configuration;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);

        builder.Services.AddHealthChecks();

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        key => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 15,
                            Window = TimeSpan.FromSeconds(10),
                        }
                    )
            );

            options.OnRejected = async (context, cancellationToken) =>
            {
                var response = context.HttpContext.Response;

                response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (!response.HasStarted)
                {
                    await response.WriteAsync("Too Many Requests", cancellationToken);
                }
            };
        });

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
