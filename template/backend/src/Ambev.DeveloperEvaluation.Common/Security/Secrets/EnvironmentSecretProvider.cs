using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.Common.Security.Secrets;

public sealed class EnvironmentSecretProvider : ISecretProvider
{
    private static readonly string[] KnownKeys =
    [
        "Jwt:SecretKey",
        "Jwt:Issuer",
        "Jwt:Audience",
        "ConnectionStrings:DefaultConnection"
    ];

    private readonly IConfiguration _configuration;

    public EnvironmentSecretProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string?> GetSecretAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_configuration[key]);
    }

    public async Task<IDictionary<string, string>> GetSecretsAsync(CancellationToken cancellationToken)
    {
        var secrets = new Dictionary<string, string>();
        foreach (var key in KnownKeys)
        {
            var value = await GetSecretAsync(key, cancellationToken);
            if (!string.IsNullOrWhiteSpace(value))
                secrets[key] = value;
        }

        return secrets;
    }
}
