using Ambev.DeveloperEvaluation.Common.Security.Secrets;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ambev.DeveloperEvaluation.WebApi.Configuration;

public static class VaultConfigurationExtensions
{
    public static WebApplicationBuilder AddDeveloperStoreSecrets(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection("Vault").Get<VaultOptions>() ?? new VaultOptions();
        builder.Services.Configure<VaultOptions>(builder.Configuration.GetSection("Vault"));
        builder.Services.AddSingleton(options);
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<EnvironmentSecretProvider>();
        builder.Services.AddSingleton(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<VaultSecretProvider>>();
            return new VaultSecretProvider(httpClientFactory.CreateClient(nameof(VaultSecretProvider)), options, logger);
        });
        builder.Services.AddSingleton<ISecretProvider, CompositeSecretProvider>();

        if (!options.Enabled)
            return builder;

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Math.Max(options.TimeoutSeconds, 1)));
        var provider = CreateStartupProvider(builder, options);

        try
        {
            var secrets = provider.GetSecretsAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
            if (secrets.Count > 0)
                builder.Configuration.AddInMemoryCollection(secrets.Select(secret => new KeyValuePair<string, string?>(secret.Key, secret.Value)));
        }
        catch (Exception exception) when (builder.Environment.IsDevelopment())
        {
            Console.Error.WriteLine($"Vault local indisponivel em Development. Fallback para appsettings/env vars. Erro: {exception.GetType().Name}");
        }

        return builder;
    }

    private static ISecretProvider CreateStartupProvider(WebApplicationBuilder builder, VaultOptions options)
    {
        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(Math.Max(options.TimeoutSeconds, 1))
        };
        var vaultProvider = new VaultSecretProvider(httpClient, options, NullLogger<VaultSecretProvider>.Instance);
        var environmentProvider = new EnvironmentSecretProvider(builder.Configuration);
        return new CompositeSecretProvider(
            vaultProvider,
            environmentProvider,
            builder.Environment,
            options,
            NullLogger<CompositeSecretProvider>.Instance);
    }
}
