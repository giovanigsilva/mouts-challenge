using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Common.Security.Secrets;

public sealed class VaultSecretProvider : ISecretProvider
{
    private readonly HttpClient _httpClient;
    private readonly VaultOptions _options;
    private readonly ILogger<VaultSecretProvider> _logger;

    public VaultSecretProvider(HttpClient httpClient, VaultOptions options, ILogger<VaultSecretProvider> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<string?> GetSecretAsync(string key, CancellationToken cancellationToken)
    {
        var secrets = await GetSecretsAsync(cancellationToken);
        return secrets.TryGetValue(NormalizeKey(key), out var value) ? value : null;
    }

    public async Task<IDictionary<string, string>> GetSecretsAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
            return new Dictionary<string, string>();

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, BuildVaultUri());
            request.Headers.Add("X-Vault-Token", _options.Token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var data = document.RootElement.GetProperty("data").GetProperty("data");
            var secrets = new Dictionary<string, string>();
            foreach (var property in data.EnumerateObject())
            {
                var value = property.Value.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    secrets[NormalizeKey(property.Name)] = value;
            }

            _logger.LogInformation("Segredos carregados do Vault. Mount={VaultMount} Path={VaultPath} Count={SecretCount}", _options.Mount, _options.Path, secrets.Count);
            return secrets;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Falha ao carregar segredos do Vault. Mount={VaultMount} Path={VaultPath}", _options.Mount, _options.Path);
            throw;
        }
    }

    private Uri BuildVaultUri()
    {
        var address = _options.Address.TrimEnd('/');
        var mount = _options.Mount.Trim('/');
        var path = _options.Path.Trim('/');
        return new Uri($"{address}/v1/{mount}/data/{path}");
    }

    private static string NormalizeKey(string key)
    {
        return key.Replace("__", ":", StringComparison.Ordinal);
    }
}
