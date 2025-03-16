namespace Server;

using Server.Configuration;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        app.UseCors();

        app.MapGraphQL();

        app.Run();
    }
}
