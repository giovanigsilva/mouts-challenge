# Key Vault

Use Azure Key Vault references in App Service and Function App settings.

Examples:

```text
Jwt__SecretKey=@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/JwtSecretKey/)
ConnectionStrings__DefaultConnection=@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/PostgresConnectionString/)
ConnectionStrings__Redis=@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/RedisConnectionString/)
ServiceBus__ConnectionString=@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/ServiceBusConnectionString/)
APPLICATIONINSIGHTS_CONNECTION_STRING=@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/AppInsightsConnectionString/)
DD_API_KEY=@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/DatadogApiKey/)
```

Recommended permissions:

- API/Worker: Key Vault Secrets User.
- API/Worker: Service Bus Data Sender.
- Functions: Service Bus Data Receiver.

Never commit:

- real connection strings
- JWT secrets
- Datadog API keys
- Application Insights connection strings
- publish profiles

