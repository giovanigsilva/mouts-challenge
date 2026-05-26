# DeveloperStore Sales API

API .NET 8 do desafio DeveloperStore com Auth, Users, Sales, Swagger em portugues, PostgreSQL, Docker, observabilidade com Datadog/Seq e segredos locais via HashiCorp Vault em Development.

## Rodar Com Docker

```powershell
docker compose up -d ambev.developerevaluation.database vault seq
$env:VAULT_ADDR="http://localhost:8200"
$env:VAULT_TOKEN="dev-root-token"
.\scripts\vault-init-dev.ps1
docker compose up --build ambev.developerevaluation.webapi
```

Swagger:

```text
http://localhost:8080/swagger
```

Health:

```powershell
curl -i http://localhost:8080/health/live
curl -i http://localhost:8080/health/ready
```

## Segredos

Development pode usar Vault local em modo dev. Os valores commitados sao apenas `DevOnly` para a prova local e nao devem ser usados em Production.

Production exige variaveis de ambiente reais:

- `ConnectionStrings__DefaultConnection`
- `Jwt__SecretKey`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Security__AllowedOrigins__0`

Em producao real, usar Azure Key Vault References ou variaveis seguras do ambiente hospedado. Nunca commitar senha real, JWT secret real, token real ou connection string real.

## Comandos

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

## Ambientes

- Development: Swagger e detailed errors habilitados, Vault local opcional, fallback controlado para appsettings/env vars.
- UAT: Swagger habilitado para avaliacao, detailed errors desabilitados, CORS restrito.
- Production: Swagger desabilitado por padrao, detailed errors desabilitados, CORS restrito e validação forte de segredos no startup.
