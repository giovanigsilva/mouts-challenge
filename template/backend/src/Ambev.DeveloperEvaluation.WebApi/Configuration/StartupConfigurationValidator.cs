namespace Ambev.DeveloperEvaluation.WebApi.Configuration;

public static class StartupConfigurationValidator
{
    public static void Validate(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            ValidateDevelopment(builder.Configuration);
            return;
        }

        if (builder.Environment.IsEnvironment("Uat"))
        {
            ValidateUat(builder.Configuration);
            return;
        }

        if (builder.Environment.IsProduction())
        {
            ValidateProduction(builder.Configuration);
            return;
        }
    }

    private static void ValidateDevelopment(IConfiguration configuration)
    {
        ValidateRequired(configuration.GetConnectionString("DefaultConnection"), "ConnectionStrings:DefaultConnection");
        ValidateRequired(configuration["Jwt:SecretKey"], "Jwt:SecretKey");
        ValidateRequired(configuration["Jwt:Issuer"], "Jwt:Issuer");
        ValidateRequired(configuration["Jwt:Audience"], "Jwt:Audience");
    }

    private static void ValidateUat(IConfiguration configuration)
    {
        ValidateRequired(configuration.GetConnectionString("DefaultConnection"), "ConnectionStrings:DefaultConnection");
        ValidateRequired(configuration["Jwt:SecretKey"], "Jwt:SecretKey");
        ValidateRequired(configuration["Jwt:Issuer"], "Jwt:Issuer");
        ValidateRequired(configuration["Jwt:Audience"], "Jwt:Audience");
        ValidateStrongJwtSecret(configuration["Jwt:SecretKey"], "Uat");
        ValidateCors(configuration, "Uat");

        if (configuration.GetValue<bool>("Features:EnableDetailedErrors"))
            throw new InvalidOperationException("Features:EnableDetailedErrors deve estar desabilitado em Uat.");
    }

    private static void ValidateProduction(IConfiguration configuration)
    {
        ValidateRequired(configuration.GetConnectionString("DefaultConnection"), "ConnectionStrings:DefaultConnection");
        ValidateRequired(configuration["Jwt:SecretKey"], "Jwt:SecretKey");
        ValidateRequired(configuration["Jwt:Issuer"], "Jwt:Issuer");
        ValidateRequired(configuration["Jwt:Audience"], "Jwt:Audience");
        ValidateStrongJwtSecret(configuration["Jwt:SecretKey"], "Production");
        ValidateCors(configuration, "Production");
        ValidateNotDevelopmentOnly(configuration);

        if (configuration.GetValue<bool>("Swagger:Enabled"))
            throw new InvalidOperationException("Swagger deve ficar desabilitado por padrao em Production.");

        if (configuration.GetValue<bool>("Features:EnableDetailedErrors"))
            throw new InvalidOperationException("Features:EnableDetailedErrors deve estar desabilitado em Production.");

        if (configuration["Vault:Token"]?.Equals("dev-root-token", StringComparison.OrdinalIgnoreCase) == true)
            throw new InvalidOperationException("Vault:Token de desenvolvimento nao pode ser usado em Production.");
    }

    private static void ValidateRequired(string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Configuracao obrigatoria ausente: {key}");
    }

    private static void ValidateStrongJwtSecret(string? secretKey, string environmentName)
    {
        if (secretKey!.Length < 32)
            throw new InvalidOperationException($"Configuracao Jwt:SecretKey deve possuir pelo menos 32 caracteres em {environmentName}.");

        if (secretKey.Contains("DevelopmentOnly", StringComparison.OrdinalIgnoreCase) ||
            secretKey.Contains("DevOnly", StringComparison.OrdinalIgnoreCase) ||
            secretKey.Contains("ChangeMe", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Configuracao Jwt:SecretKey nao pode usar valor local ou placeholder inseguro em {environmentName}.");
    }

    private static void ValidateCors(IConfiguration configuration, string environmentName)
    {
        var origins = configuration.GetSection("Security:AllowedOrigins").Get<string[]>() ?? [];
        if (origins.Length == 0 || origins.Any(origin => origin == "*"))
            throw new InvalidOperationException($"Security:AllowedOrigins deve ser restrito em {environmentName}.");
    }

    private static void ValidateNotDevelopmentOnly(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var forbiddenFragments = new[]
        {
            "Pass@word",
            "developerstore_app",
            "developerstore_uat_app",
            "DevOnly",
            "UatOnly",
            "ReplaceWith",
            "localhost",
            "ambev.developerevaluation.database"
        };

        if (forbiddenFragments.Any(fragment => connectionString.Contains(fragment, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection nao pode usar valor local, UAT ou placeholder em Production.");
    }
}
