# Ambientes

## Development

Usado na branch `develop`.

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Swagger e detailed errors ficam habilitados.

## UAT

Usado para homologacao simulada.

```powershell
$env:ASPNETCORE_ENVIRONMENT="Uat"
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
$env:Jwt__SecretKey="UatOnlySecretKeyForDeveloperStoreApi123456789"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Swagger fica habilitado para avaliacao. Detailed errors ficam desabilitados.

## Production

Usado na branch `main`.

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=troque-esta-senha"
$env:Jwt__SecretKey="ProductionSimulationSecretKeyWithAtLeast32Chars"
$env:Jwt__Issuer="DeveloperStore"
$env:Jwt__Audience="DeveloperStore"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Production nao deve conter segredo real em arquivo.
