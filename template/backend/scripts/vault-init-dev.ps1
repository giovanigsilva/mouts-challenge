$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($env:VAULT_ADDR)) {
    $env:VAULT_ADDR = "http://localhost:8200"
}

if ([string]::IsNullOrWhiteSpace($env:VAULT_TOKEN)) {
    $env:VAULT_TOKEN = "dev-root-token"
}

vault kv put secret/developerstore/development `
  "Jwt__SecretKey=DevOnly_JwtSecret_2026_pZ9!mQ4#vL8@rT2%DeveloperStore" `
  "Jwt__Issuer=DeveloperStore.Development" `
  "Jwt__Audience=DeveloperStore.Api" `
  "ConnectionStrings__DefaultConnection=Host=ambev.developerevaluation.database;Port=5432;Database=developer_evaluation;Username=developerstore_app;Password=DevOnly_Pg_9fR!42sL#2026_Strong"
