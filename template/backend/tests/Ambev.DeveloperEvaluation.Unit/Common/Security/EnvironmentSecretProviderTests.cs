using Ambev.DeveloperEvaluation.Common.Security.Secrets;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Security;

public sealed class EnvironmentSecretProviderTests
{
    [Fact(DisplayName = "Given configuration secret When provider reads key Then returns value")]
    public async Task GetSecretAsync_ConfiguredKey_ReturnsValue()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "DeveloperStore.Development"
            })
            .Build();
        var provider = new EnvironmentSecretProvider(configuration);

        var result = await provider.GetSecretAsync("Jwt:Issuer", CancellationToken.None);

        result.Should().Be("DeveloperStore.Development");
    }

    [Fact(DisplayName = "Given known configured keys When provider reads all Then returns only configured secrets")]
    public async Task GetSecretsAsync_ConfiguredKnownKeys_ReturnsConfiguredSecrets()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "DeveloperStore.Development",
                ["Jwt:Audience"] = "DeveloperStore.Api"
            })
            .Build();
        var provider = new EnvironmentSecretProvider(configuration);

        var result = await provider.GetSecretsAsync(CancellationToken.None);

        result.Should().ContainKey("Jwt:Issuer");
        result.Should().ContainKey("Jwt:Audience");
        result.Should().NotContainKey("Jwt:SecretKey");
    }
}
