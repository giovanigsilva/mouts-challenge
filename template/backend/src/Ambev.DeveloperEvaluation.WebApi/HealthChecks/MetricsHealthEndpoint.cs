namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public static class MetricsHealthEndpoint
{
    public static IEndpointRouteBuilder MapMetricsHealthCheck(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health/metrics", (IConfiguration configuration, IWebHostEnvironment environment) => Results.Ok(new
        {
            status = "Healthy",
            metricsEnabled = configuration.GetValue<bool>("Metrics:Enabled"),
            prometheusEndpointEnabled = configuration.GetValue<bool>("Metrics:EnablePrometheusEndpoint"),
            environment = environment.EnvironmentName,
            serviceName = configuration["OpenTelemetry:ServiceName"] ?? configuration["Observability:ServiceName"] ?? "developerstore-sales-api"
        })).WithTags("Saude").WithName("Health_Metrics");

        return endpoints;
    }
}
