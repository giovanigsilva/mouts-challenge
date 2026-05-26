using Ambev.DeveloperEvaluation.WebApi.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

public sealed class StartupConfigurationValidatorTests
{
    [Fact(DisplayName = "Given production with empty JWT secret When validating Then throws")]
    public void Validate_ProductionEmptyJwtSecret_Throws()
    {
        var builder = CreateBuilder("Production", new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=prod-postgres.internal;Port=5432;Database=developerstore;Username=prod_app;Password=Prod_Strong_9gT!41pL#2026",
            ["Jwt:Issuer"] = "DeveloperStore.Production",
            ["Jwt:Audience"] = "DeveloperStore.Api",
            ["Security:AllowedOrigins:0"] = "https://developerstore.example.com"
        });

        Action act = () => StartupConfigurationValidator.Validate(builder);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Jwt:SecretKey*");
    }

    [Fact(DisplayName = "Given production with weak JWT secret When validating Then throws")]
    public void Validate_ProductionWeakJwtSecret_Throws()
    {
        var builder = CreateBuilder("Production", ValidProductionConfiguration());
        builder.Configuration["Jwt:SecretKey"] = "weak-secret";

        Action act = () => StartupConfigurationValidator.Validate(builder);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Jwt:SecretKey*32*");
    }

    [Fact(DisplayName = "Given production with open CORS When validating Then throws")]
    public void Validate_ProductionOpenCors_Throws()
    {
        var builder = CreateBuilder("Production", ValidProductionConfiguration());
        builder.Configuration["Security:AllowedOrigins:0"] = "*";

        Action act = () => StartupConfigurationValidator.Validate(builder);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Security:AllowedOrigins*");
    }

    [Fact(DisplayName = "Given production with development connection string When validating Then throws")]
    public void Validate_ProductionDevelopmentConnectionString_Throws()
    {
        var builder = CreateBuilder("Production", ValidProductionConfiguration());
        builder.Configuration["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=developer_evaluation;Username=developerstore_app;Password=DevOnly_Pg_9fR!42sL#2026_Strong";

        Action act = () => StartupConfigurationValidator.Validate(builder);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*DefaultConnection*");
    }

    [Fact(DisplayName = "Given development with local strong settings When validating Then does not throw")]
    public void Validate_DevelopmentValidConfiguration_DoesNotThrow()
    {
        var builder = CreateBuilder("Development", new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=developer_evaluation;Username=developerstore_app;Password=DevOnly_Pg_9fR!42sL#2026_Strong",
            ["Jwt:SecretKey"] = "DevOnly_JwtSecret_2026_pZ9!mQ4#vL8@rT2%DeveloperStore",
            ["Jwt:Issuer"] = "DeveloperStore.Development",
            ["Jwt:Audience"] = "DeveloperStore.Api"
        });

        Action act = () => StartupConfigurationValidator.Validate(builder);

        act.Should().NotThrow();
    }

    private static WebApplicationBuilder CreateBuilder(string environmentName, IDictionary<string, string?> configuration)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = environmentName
        });
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(configuration);
        return builder;
    }

    private static Dictionary<string, string?> ValidProductionConfiguration()
    {
        return new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=prod-postgres.internal;Port=5432;Database=developerstore;Username=prod_app;Password=Prod_Strong_9gT!41pL#2026",
            ["Jwt:SecretKey"] = "Prod_JwtSecret_2026_pZ9!mQ4#vL8@rT2%DeveloperStore",
            ["Jwt:Issuer"] = "DeveloperStore.Production",
            ["Jwt:Audience"] = "DeveloperStore.Api",
            ["Security:AllowedOrigins:0"] = "https://developerstore.example.com",
            ["Swagger:Enabled"] = "false",
            ["Features:EnableDetailedErrors"] = "false",
            ["Vault:Token"] = ""
        };
    }
}
