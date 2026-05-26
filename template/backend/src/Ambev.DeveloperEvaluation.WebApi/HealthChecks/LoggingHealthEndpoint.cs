namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public static class LoggingHealthEndpoint
{
    public static IEndpointRouteBuilder MapLoggingHealthCheck(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health/logging", (IConfiguration configuration, IWebHostEnvironment environment) => Results.Ok(new
        {
            datadogEnabled = configuration.GetValue<bool>("Datadog:Enabled") && configuration.GetValue<bool>("Observability:EnableDatadog"),
            seqEnabled = configuration.GetValue<bool>("Seq:Enabled") && configuration.GetValue<bool>("Observability:EnableSeqFallback"),
            seqUrlConfigured = !string.IsNullOrWhiteSpace(configuration["Seq:Url"]),
            consoleLoggingEnabled = true,
            environment = environment.EnvironmentName,
            serviceName = configuration["Observability:ServiceName"] ?? "developerstore-sales-api"
        }))
        .WithTags("Saude")
        .WithName("Health_Logging")
        .AllowAnonymous();

        return endpoints;
    }
}
