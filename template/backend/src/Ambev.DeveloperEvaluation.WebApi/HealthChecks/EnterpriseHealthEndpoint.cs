using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public static class EnterpriseHealthEndpoint
{
    public static IEndpointRouteBuilder MapEnterpriseHealthCheck(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health/enterprise", async (IConfiguration configuration, IWebHostEnvironment environment, IHttpClientFactory httpClientFactory, CancellationToken cancellationToken) =>
        {
            var services = new List<EnterpriseServiceHealth>
            {
                await CheckHttpServiceAsync(httpClientFactory, "Seq", configuration["Seq:Url"] ?? "http://seq:5341", cancellationToken),
                await CheckRedisAsync(configuration, cancellationToken),
                await CheckHttpServiceAsync(httpClientFactory, "Prometheus", "http://prometheus:9090/-/healthy", cancellationToken),
                await CheckHttpServiceAsync(httpClientFactory, "Grafana", "http://grafana:3000/api/health", cancellationToken),
                CheckDatadog(configuration)
            };

            return Results.Ok(new
            {
                status = services.All(service => service.Status == "Healthy") ? "Healthy" : "Unhealthy",
                environment = environment.EnvironmentName,
                serviceName = configuration["Observability:ServiceName"] ?? "developerstore-sales-api",
                services
            });
        })
        .WithTags("Saude")
        .WithName("Health_Enterprise")
        .AllowAnonymous();

        return endpoints;
    }

    private static async Task<EnterpriseServiceHealth> CheckHttpServiceAsync(IHttpClientFactory httpClientFactory, string name, string url, CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(2);

            using var response = await httpClient.GetAsync(url, cancellationToken);

            return new EnterpriseServiceHealth(name, "Healthy", url);
        }
        catch
        {
            return new EnterpriseServiceHealth(name, "Unhealthy", url);
        }
    }

    private static async Task<EnterpriseServiceHealth> CheckRedisAsync(IConfiguration configuration, CancellationToken cancellationToken)
    {
        var redisConnection = configuration.GetConnectionString("Redis");

        if (string.IsNullOrWhiteSpace(redisConnection))
            return new EnterpriseServiceHealth("Redis", "Unhealthy", "ConnectionStrings:Redis ausente");

        try
        {
            var options = ConfigurationOptions.Parse(redisConnection);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 1500;
            options.SyncTimeout = 1500;

            await using var connection = await ConnectionMultiplexer.ConnectAsync(options);
            await connection.GetDatabase().PingAsync(CommandFlags.None);
            cancellationToken.ThrowIfCancellationRequested();

            return new EnterpriseServiceHealth("Redis", "Healthy", "ConnectionStrings:Redis configurado");
        }
        catch
        {
            return new EnterpriseServiceHealth("Redis", "Unhealthy", "redis:6379");
        }
    }

    private static EnterpriseServiceHealth CheckDatadog(IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("Datadog:Enabled") && configuration.GetValue<bool>("Observability:EnableDatadog");

        return new EnterpriseServiceHealth("Datadog", enabled ? "Healthy" : "Unhealthy", "Agent Docker via stdout/stderr");
    }

    private sealed record EnterpriseServiceHealth(string Name, string Status, string Target);
}
