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

        if (builder.Configuration.GetValue<bool>("Cache:Enabled"))
            ValidateRequired(builder.Configuration.GetConnectionString("Redis"), "ConnectionStrings:Redis");

        if (builder.Configuration.GetValue<bool>("ServiceBus:Enabled"))
        {
            var managedIdentity = builder.Configuration.GetValue<bool>("ServiceBus:UseManagedIdentity");
            if (managedIdentity)
                ValidateRequired(builder.Configuration["ServiceBus:FullyQualifiedNamespace"], "ServiceBus:FullyQualifiedNamespace");
            else
                ValidateRequired(builder.Configuration["ServiceBus:ConnectionString"], "ServiceBus:ConnectionString");
        }
    }

    private static void ValidateRequired(string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Configuracao obrigatoria ausente: {key}");
    }
}
