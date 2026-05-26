using Ambev.DeveloperEvaluation.Common.Security.Secrets;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ambev.DeveloperEvaluation.WebApi.HealthChecks;

public sealed class VaultHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly VaultOptions _options;
    private readonly IWebHostEnvironment _environment;

    public VaultHealthCheck(IHttpClientFactory httpClientFactory, VaultOptions options, IWebHostEnvironment environment)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _environment = environment;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
            return HealthCheckResult.Healthy("Vault desabilitado.");

        if (!_environment.IsDevelopment())
            return HealthCheckResult.Unhealthy("Vault local nao deve ser usado fora de Development.");

        if (string.IsNullOrWhiteSpace(_options.Address))
            return HealthCheckResult.Unhealthy("Vault habilitado sem endereco configurado.");

        try
        {
            var client = _httpClientFactory.CreateClient(nameof(VaultHealthCheck));
            client.Timeout = TimeSpan.FromSeconds(Math.Max(_options.TimeoutSeconds, 1));
            using var response = await client.GetAsync($"{_options.Address.TrimEnd('/')}/v1/sys/health", cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("Vault local disponivel.")
                : HealthCheckResult.Unhealthy("Vault local indisponivel.");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Vault local indisponivel.");
        }
    }
}
