namespace Ambev.DeveloperEvaluation.WebApi.Configuration;

public static class StartupConfigurationValidator
{
    public static void Validate(WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsProduction())
            return;

        ValidateRequired(builder.Configuration.GetConnectionString("DefaultConnection"), "ConnectionStrings:DefaultConnection");
        ValidateRequired(builder.Configuration["Jwt:SecretKey"], "Jwt:SecretKey");
        ValidateRequired(builder.Configuration["Jwt:Issuer"], "Jwt:Issuer");
        ValidateRequired(builder.Configuration["Jwt:Audience"], "Jwt:Audience");

        var secretKey = builder.Configuration["Jwt:SecretKey"];
        if (secretKey!.Length < 32)
            throw new InvalidOperationException("Configuracao Jwt:SecretKey deve possuir pelo menos 32 caracteres em Production.");

        if (builder.Configuration.GetConnectionString("DefaultConnection")!.Contains("Pass@word", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection nao pode usar senha local padrao em Production.");
    }

    private static void ValidateRequired(string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Configuracao obrigatoria ausente: {key}");
    }
}
