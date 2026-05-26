using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public sealed class SecurityConfigurationHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public SecurityConfigurationHealthCheck(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var missingKeys = new List<string>();

        AddIfMissing(missingKeys, _configuration.GetConnectionString("DefaultConnection"), "ConnectionStrings:DefaultConnection");
        AddIfMissing(missingKeys, _configuration["Jwt:SecretKey"], "Jwt:SecretKey");
        AddIfMissing(missingKeys, _configuration["Jwt:Issuer"], "Jwt:Issuer");
        AddIfMissing(missingKeys, _configuration["Jwt:Audience"], "Jwt:Audience");

        if (missingKeys.Count > 0)
            return Task.FromResult(HealthCheckResult.Unhealthy("Configuracoes criticas ausentes.", data: new Dictionary<string, object> { ["missingKeys"] = missingKeys }));

        if (!_environment.IsDevelopment())
        {
            var origins = _configuration.GetSection("Security:AllowedOrigins").Get<string[]>() ?? [];
            if (origins.Length == 0 || origins.Any(origin => origin == "*"))
                return Task.FromResult(HealthCheckResult.Unhealthy("CORS restrito nao configurado."));
        }

        return Task.FromResult(HealthCheckResult.Healthy("Configuracoes criticas presentes."));
    }

    private static void AddIfMissing(ICollection<string> keys, string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
            keys.Add(key);
    }
}
