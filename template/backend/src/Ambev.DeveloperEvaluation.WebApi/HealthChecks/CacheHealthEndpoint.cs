using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public static class CacheHealthEndpoint
{
    public static IEndpointRouteBuilder MapCacheHealthCheck(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health/cache", async (IConfiguration configuration, IWebHostEnvironment environment) =>
        {
            var enabled = configuration.GetValue<bool>("Cache:Enabled");
            var redisConnection = configuration.GetConnectionString("Redis");

            if (!enabled)
            {
                return Results.Ok(new
                {
                    status = "Healthy",
                    cacheEnabled = false,
                    redisConfigured = !string.IsNullOrWhiteSpace(redisConnection),
                    environment = environment.EnvironmentName
                });
            }

            if (string.IsNullOrWhiteSpace(redisConnection))
            {
                return Results.Json(new
                {
                    status = "Unhealthy",
                    cacheEnabled = true,
                    redisConfigured = false,
                    environment = environment.EnvironmentName
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            try
            {
                var options = ConfigurationOptions.Parse(redisConnection);
                options.AbortOnConnectFail = false;
                options.ConnectTimeout = 1500;
                options.SyncTimeout = 1500;

                await using var connection = await ConnectionMultiplexer.ConnectAsync(options);
                var latency = await connection.GetDatabase().PingAsync();

                return Results.Ok(new
                {
                    status = "Healthy",
                    cacheEnabled = true,
                    redisConfigured = true,
                    redisReachable = true,
                    latencyMs = latency.TotalMilliseconds,
                    environment = environment.EnvironmentName
                });
            }
            catch
            {
                return Results.Json(new
                {
                    status = "Unhealthy",
                    cacheEnabled = true,
                    redisConfigured = true,
                    redisReachable = false,
                    environment = environment.EnvironmentName
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }).WithTags("Saude").WithName("Health_Cache");

        return endpoints;
    }
}
