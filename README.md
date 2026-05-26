# DeveloperStore Sales API

API de vendas do desafio DeveloperStore, evoluida a partir do template Ambev.DeveloperEvaluation em .NET 8.

## Visao Geral

O projeto implementa um CRUD completo de vendas usando Clean Architecture proxima do template original:

- WebApi: endpoints HTTP, Swagger, autenticacao/autorizacao, middlewares e health checks.
- Application: casos de uso com MediatR, validators, handlers, results e profiles.
- Domain: entidades, aggregate Sales, regras de negocio, politica de desconto, eventos de dominio e contratos de repositorio.
- ORM: EF Core com PostgreSQL, mappings, repositories e migrations.
- Common: seguranca, JWT, validacao, logging e health checks compartilhados.
- IoC: registro de dependencias.

Auth e Users originais foram preservados. Sales foi adicionado sem Azure real, Service Bus real, Redis obrigatorio, Key Vault real, worker real ou filas reais. Os eventos diferenciais sao registrados no log da aplicacao.

## Backend

```powershell
cd template/backend
```

Projetos principais:

```text
src/Ambev.DeveloperEvaluation.WebApi
src/Ambev.DeveloperEvaluation.Application
src/Ambev.DeveloperEvaluation.Domain
src/Ambev.DeveloperEvaluation.ORM
src/Ambev.DeveloperEvaluation.Common
src/Ambev.DeveloperEvaluation.IoC
tests/Ambev.DeveloperEvaluation.Unit
tests/Ambev.DeveloperEvaluation.Integration
tests/Ambev.DeveloperEvaluation.Functional
```

## Tecnologias

.NET 8, ASP.NET Core, MediatR, AutoMapper, FluentValidation, EF Core, PostgreSQL/Npgsql, JWT Bearer, BCrypt, Serilog, Swagger/OpenAPI, xUnit, FluentAssertions e Docker Compose.

## Regras de Vendas

Sales usa External Identities com snapshot denormalizado:

- Sale armazena `CustomerExternalId`, `CustomerName`, `BranchExternalId`, `BranchName`.
- SaleItem armazena `ProductExternalId`, `ProductName`.
- Nao foram criados dominios reais de Customer, Branch ou Product.

Politica de desconto:

- 1 a 3 itens identicos: 0%.
- 4 a 9 itens identicos: 10%.
- 10 a 20 itens identicos: 20%.
- Acima de 20 itens identicos: erro de regra de negocio.
- Produto duplicado na mesma venda e rejeitado.
- Item cancelado nao compoe o total financeiro.
- Venda cancelada nao permite alteracao de itens.
- O total da venda e sempre recalculado pelo aggregate no Domain.

## Endpoints Sales

Todos os endpoints de Sales exigem JWT.

```text
POST   /api/sales
GET    /api/sales
GET    /api/sales/{id}
PUT    /api/sales/{id}
DELETE /api/sales/{id}
PATCH  /api/sales/{id}/cancel
PATCH  /api/sales/{id}/items/{itemId}/cancel
```

Filtros da listagem: `page`, `pageSize`, `saleNumber`, `customerId`, `branchId`, `isCancelled`, `fromDate`, `toDate`.

DELETE remove fisicamente a venda. Cancelamento de venda deve usar `PATCH /api/sales/{id}/cancel`.

## Eventos em Log

Nao ha broker real nesta entrega simplificada. Os eventos `SaleCreated`, `SaleModified`, `SaleCancelled` e `ItemCancelled` sao registrados no application log.

## Ambientes

Arquivos:

```text
src/Ambev.DeveloperEvaluation.WebApi/appsettings.json
src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json
src/Ambev.DeveloperEvaluation.WebApi/appsettings.Uat.json
src/Ambev.DeveloperEvaluation.WebApi/appsettings.Production.json
```

Mapeamento:

```text
develop -> ASPNETCORE_ENVIRONMENT=Development
uat     -> ASPNETCORE_ENVIRONMENT=Uat
main    -> ASPNETCORE_ENVIRONMENT=Production
```

Production nao contem segredo real. Em producao real, sobrescreva configuracoes por variaveis de ambiente ou Key Vault.

Variaveis principais:

```text
ASPNETCORE_ENVIRONMENT
ConnectionStrings__DefaultConnection
Jwt__SecretKey
Jwt__Issuer
Jwt__Audience
Jwt__ExpirationMinutes
Swagger__Enabled
Security__AllowedOrigins__0
Security__EnableHsts
Security__EnableHttpsRedirection
Security__EnableSecurityHeaders
Features__EnableDetailedErrors
```

## Executar Localmente

```powershell
cd template/backend
docker compose up -d ambev.developerevaluation.database
dotnet restore
dotnet build --configuration Release --no-restore
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Swagger local:

```text
http://localhost:5119/swagger
```

Docker completo:

```powershell
docker compose up --build
```

Swagger no Docker:

```text
http://localhost:8080/swagger
```

## Simular UAT

```powershell
$env:ASPNETCORE_ENVIRONMENT="Uat"
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
$env:Jwt__SecretKey="UatOnlySecretKeyForDeveloperStoreApi123456789"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

## Simular Production

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=troque-esta-senha"
$env:Jwt__SecretKey="ProductionSimulationSecretKeyWithAtLeast32Chars"
$env:Jwt__Issuer="DeveloperStore"
$env:Jwt__Audience="DeveloperStore"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Em Production, o startup falha se `Jwt:SecretKey`, issuer, audience ou connection string obrigatoria estiverem ausentes ou inseguros.

## Testes

```powershell
dotnet test --configuration Release
```

Cobertura adicionada: descontos por quantidade, limite de 20 itens, produto duplicado, item cancelado fora do total, venda cancelada bloqueando alteracao, eventos de dominio e validators de Sales.

## Health Checks

```powershell
curl -i http://localhost:8080/health/live
curl -i http://localhost:8080/health/ready
```

`/health/live` verifica processo vivo. `/health/ready` verifica readiness e PostgreSQL.

## Autenticacao no Swagger

1. Crie ou autentique um usuario pelos endpoints existentes de Auth/Users.
2. Copie o token JWT retornado.
3. Clique em `Authorize`.
4. Informe `Bearer {token}`.

## Exemplo de Criacao de Venda

```powershell
curl -X POST "http://localhost:8080/api/sales" `
  -H "Authorization: Bearer {token}" `
  -H "Content-Type: application/json" `
  -d '{
    "saleNumber": "SALE-2026-000001",
    "saleDate": "2026-05-26T12:00:00Z",
    "customerExternalId": "11111111-1111-1111-1111-111111111111",
    "customerName": "Cliente Exemplo",
    "branchExternalId": "22222222-2222-2222-2222-222222222222",
    "branchName": "Filial Centro",
    "items": [
      {
        "productExternalId": "33333333-3333-3333-3333-333333333333",
        "productName": "Produto Exemplo",
        "quantity": 10,
        "unitPrice": 100
      }
    ]
  }'
```

## Seguranca Aplicada

- JWT Bearer com validacao de issuer, audience, lifetime e signing key.
- Secret JWT fraco rejeitado em Production.
- Policies `Sales.Read`, `Sales.Write`, `Sales.Cancel`, `Sales.Delete`.
- CORS por ambiente.
- HTTPS/HSTS controlados por configuracao.
- Security headers basicos.
- GlobalExceptionMiddleware com resposta padronizada e correlationId.
- CorrelationIdMiddleware com header `X-Correlation-Id`.
- Logs sem expor segredos.

## Commits por Etapa

Os commits seguem o padrao `feature: texto curto explicando a etapa`.

## Checklist

- Branch `develop`.
- Sales CRUD completo.
- Regras de desconto no Domain.
- Aggregate Sale protegido.
- External Identities por snapshot.
- PostgreSQL via EF Core.
- Migration de Sales e SaleItems.
- Eventos registrados em log.
- Swagger em PT-BR para Sales.
- Ambientes Development, UAT e Production.
- Health checks live e ready.
- Testes unitarios passando.
- Docker Compose simples com WebApi e PostgreSQL.
- README com configuracao, execucao e teste.

## Fora do Escopo Simplificado

Nao foram implementados Azure Service Bus, Redis obrigatorio, Azure Functions, Outbox, Key Vault real, worker real, filas reais ou blue-green real, conforme escopo simplificado da prova.
