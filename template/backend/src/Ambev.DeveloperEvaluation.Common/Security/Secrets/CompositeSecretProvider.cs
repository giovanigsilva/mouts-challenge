using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Common.Security.Secrets;

public sealed class CompositeSecretProvider : ISecretProvider
{
    private readonly VaultSecretProvider _vaultSecretProvider;
    private readonly EnvironmentSecretProvider _environmentSecretProvider;
    private readonly IHostEnvironment _environment;
    private readonly VaultOptions _options;
    private readonly ILogger<CompositeSecretProvider> _logger;

    public CompositeSecretProvider(
        VaultSecretProvider vaultSecretProvider,
        EnvironmentSecretProvider environmentSecretProvider,
        IHostEnvironment environment,
        VaultOptions options,
        ILogger<CompositeSecretProvider> logger)
    {
        _vaultSecretProvider = vaultSecretProvider;
        _environmentSecretProvider = environmentSecretProvider;
        _environment = environment;
        _options = options;
        _logger = logger;
    }

    public async Task<string?> GetSecretAsync(string key, CancellationToken cancellationToken)
    {
        var secrets = await GetSecretsAsync(cancellationToken);
        return secrets.TryGetValue(key, out var value) ? value : null;
    }

    public async Task<IDictionary<string, string>> GetSecretsAsync(CancellationToken cancellationToken)
    {
        if (_options.Enabled)
        {
            try
            {
                return await _vaultSecretProvider.GetSecretsAsync(cancellationToken);
            }
            catch when (_environment.IsDevelopment())
            {
                _logger.LogWarning("Vault indisponivel em Development. Fallback controlado para configuracao local sem expor valores.");
            }
            catch when (!_environment.IsProduction() && !_options.FailFastInProduction)
            {
                _logger.LogWarning("Vault indisponivel fora de Production. Fallback controlado para configuracao local sem expor valores.");
            }
        }

        return await _environmentSecretProvider.GetSecretsAsync(cancellationToken);
    }
}
