using Ambev.DeveloperEvaluation.WebApi.Configuration;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDeveloperStoreSecrets();
            StartupConfigurationValidator.Validate(builder);
            builder.AddWebApiServices();

            var app = builder.Build();
            await app.ApplyDatabaseMigrationsAsync();
            app.UseWebApiPipeline();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
