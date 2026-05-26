using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public sealed class PostgreSqlHealthCheck : IHealthCheck
{
    private readonly DefaultContext _context;

    public PostgreSqlHealthCheck(DefaultContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            return canConnect ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("PostgreSQL indisponivel.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL indisponivel.", ex);
        }
    }
}
