using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly DefaultContext _context;

    public DatabaseHealthCheck(DefaultContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("PostgreSQL disponivel")
                : HealthCheckResult.Unhealthy("PostgreSQL indisponivel");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL indisponivel", exception);
        }
    }
}
