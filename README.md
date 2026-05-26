# DeveloperStore Sales API

Backend do desafio DeveloperStore implementado sobre o template `Ambev.DeveloperEvaluation`, usando .NET 8, Clean Architecture, DDD no dominio de vendas, CQRS com MediatR, EF Core com PostgreSQL, JWT, FluentValidation, AutoMapper, Serilog, Swagger/OpenAPI, Docker e testes unitarios.

Este README descreve exatamente o que existe nesta branch.

## Status da Entrega

Implementado:

- Auth e Users preservados do template.
- CRUD completo de Sales.
- Regras de desconto no Domain.
- Aggregate `Sale` com itens protegidos.
- External Identities com snapshot denormalizado para cliente, filial e produto.
- EF Core/PostgreSQL com migration de Sales.
- Eventos diferenciais registrados em log.
- Swagger habilitado em Development e UAT.
- Separacao de ambientes Development, UAT e Production.
- Health checks `/health/live` e `/health/ready`.
- Docker Compose com WebApi e PostgreSQL.
- Testes unitarios de regras de dominio e validators.
- Documentacao complementar na pasta `.doc` e em `template/backend/docs`.

Fora do escopo implementado nesta versao:

- Azure Service Bus real.
- Redis obrigatorio.
- Azure Functions.
- Worker.
- Outbox Pattern.
- Key Vault real.
- Blue-green deployment real.
- Filas reais.

Os eventos de vendas sao registrados no log da aplicacao, conforme permitido pelo enunciado da prova.

## Requisitos

Instale antes de executar:

- .NET SDK 8
- Docker Desktop
- Git
- dotnet-ef, caso ainda nao tenha:

```powershell
dotnet tool install --global dotnet-ef
```

Verifique:

```powershell
dotnet --version
docker --version
docker compose version
dotnet ef --version
```

## Estrutura

Backend:

```text
template/backend
```

Solution:

```text
template/backend/Ambev.DeveloperEvaluation.sln
```

Projetos:

```text
template/backend/src/Ambev.DeveloperEvaluation.WebApi
template/backend/src/Ambev.DeveloperEvaluation.Application
template/backend/src/Ambev.DeveloperEvaluation.Domain
template/backend/src/Ambev.DeveloperEvaluation.ORM
template/backend/src/Ambev.DeveloperEvaluation.Common
template/backend/src/Ambev.DeveloperEvaluation.IoC
```

Testes:

```text
template/backend/tests/Ambev.DeveloperEvaluation.Unit
template/backend/tests/Ambev.DeveloperEvaluation.Integration
template/backend/tests/Ambev.DeveloperEvaluation.Functional
```

Documentacao:

```text
.doc
template/backend/docs
```

## Arquitetura

A solution segue a organizacao original do template:

- `WebApi`: Controllers, requests, validators HTTP, middlewares, Swagger e health checks.
- `Application`: Commands, queries, handlers, validators, results e profiles.
- `Domain`: Entidades, regras de negocio, eventos, exceptions, services e repositories contracts.
- `ORM`: `DefaultContext`, mappings EF Core, repositories e migrations.
- `Common`: JWT, seguranca, logging, health checks e validacao.
- `IoC`: registro de dependencias.

Regra importante aplicada:

- Controller nao contem regra de negocio.
- Handler nao calcula desconto.
- Repository nao decide regra de negocio.
- Banco nao calcula regra de negocio.
- Desconto, cancelamento e totalizacao ficam no Domain.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- MediatR
- AutoMapper
- FluentValidation
- Entity Framework Core
- Npgsql/PostgreSQL
- JWT Bearer
- BCrypt
- Serilog
- Swagger/OpenAPI com Swashbuckle
- xUnit
- FluentAssertions
- Docker Compose

## Modelo Sales

`Sale`:

- `Id`
- `SaleNumber`
- `SaleDate`
- `CustomerExternalId`
- `CustomerName`
- `BranchExternalId`
- `BranchName`
- `TotalAmount`
- `IsCancelled`
- `CreatedAt`
- `UpdatedAt`
- `Items`

`SaleItem`:

- `Id`
- `SaleId`
- `ProductExternalId`
- `ProductName`
- `Quantity`
- `UnitPrice`
- `DiscountPercentage`
- `DiscountAmount`
- `TotalAmount`
- `IsCancelled`
- `CreatedAt`
- `UpdatedAt`

## Regras de Negocio

Descontos:

```text
1 a 3 itens identicos   -> 0%
4 a 9 itens identicos   -> 10%
10 a 20 itens identicos -> 20%
acima de 20             -> invalido
```

Outras regras:

- Nao e permitido repetir o mesmo `ProductExternalId` na mesma venda.
- Item cancelado nao entra no total da venda.
- Venda cancelada nao permite alteracao de itens.
- O total da venda e recalculado pelo aggregate.
- Desconto manual nao e aceito na request.

## Endpoints

Auth e Users do template:

```text
POST   /api/Auth
POST   /api/Users
GET    /api/Users/{id}
DELETE /api/Users/{id}
```

Sales:

```text
POST   /api/sales
GET    /api/sales
GET    /api/sales/{id}
PUT    /api/sales/{id}
DELETE /api/sales/{id}
PATCH  /api/sales/{id}/cancel
PATCH  /api/sales/{id}/items/{itemId}/cancel
```

Filtros de `GET /api/sales`:

```text
page
pageSize
saleNumber
customerId
branchId
isCancelled
fromDate
toDate
```

## Autenticacao

Sales exige JWT.

Fluxo:

1. Criar usuario em `POST /api/Users`.
2. Autenticar em `POST /api/Auth`.
3. Usar o token no header:

```text
Authorization: Bearer {token}
```

Policies configuradas:

```text
Sales.Read
Sales.Write
Sales.Cancel
Sales.Delete
```

Nesta entrega, as policies exigem usuario autenticado.

## Ambientes

Arquivos:

```text
template/backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.json
template/backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json
template/backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.Uat.json
template/backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.Production.json
```

Mapeamento:

```text
develop -> Development
uat     -> Uat
main    -> Production
```

Production nao possui segredo real em arquivo. Use variaveis de ambiente para connection string e JWT secret.

Variaveis suportadas:

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

## Rodar com Docker

Entre na pasta do backend:

```powershell
cd template/backend
```

Suba WebApi e PostgreSQL:

```powershell
docker compose up --build
```

Em outro terminal, aplique as migrations:

```powershell
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

URLs no Docker:

```text
API:     http://localhost:8080
Swagger: http://localhost:8080/swagger
Live:    http://localhost:8080/health/live
Ready:   http://localhost:8080/health/ready
```

Parar containers:

```powershell
docker compose down
```

Remover volumes do banco local:

```powershell
docker compose down -v
```

## Rodar Local Sem Container da API

Suba apenas PostgreSQL:

```powershell
cd template/backend
docker compose up -d ambev.developerevaluation.database
```

Configure ambiente:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
```

Restaure, compile e aplique migrations:

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Execute a API:

```powershell
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

URL local comum:

```text
http://localhost:5119/swagger
```

A porta exata tambem aparece no output do `dotnet run`.

## Rodar Testes

Na pasta `template/backend`:

```powershell
dotnet test --configuration Release
```

Resultado validado nesta branch:

```text
71 testes unitarios passando
0 falhas
```

Observacao: os projetos `Functional` e `Integration` existem no template, mas atualmente nao possuem testes descobertos.

## Verificar Vulnerabilidades de Pacotes

O comando na solution pode retornar erro por causa do projeto `docker-compose.dcproj`. Para validar os projetos .NET reais:

```powershell
$projects = Get-ChildItem -Path .\src, .\tests -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }
foreach ($project in $projects) {
  dotnet list $project.FullName package --vulnerable --include-transitive
}
```

Validado nesta branch: nenhum pacote vulneravel nos `.csproj` da solution.

## Health Checks

Com Docker:

```powershell
curl.exe -i http://localhost:8080/health/live
curl.exe -i http://localhost:8080/health/ready
```

`/health/live` valida processo vivo.

`/health/ready` valida readiness e conectividade com PostgreSQL.

## Documentacao Swagger/OpenAPI

Docker:

```text
http://localhost:8080/swagger
```

Local via `dotnet run`:

```text
http://localhost:5119/swagger
```

No Swagger:

1. Crie usuario em `POST /api/Users`.
2. Autentique em `POST /api/Auth`.
3. Copie `data.token`.
4. Clique em `Authorize`.
5. Informe `Bearer {token}`.

O Swagger foi configurado para funcionar como documentacao principal da API:

- Titulo: `DeveloperStore Sales API`.
- Descricao em portugues do Brasil com arquitetura, Sales, regras de desconto, eventos logados, ambientes, health checks e padrao de resposta.
- Botao `Authorize` com JWT Bearer.
- Header opcional `X-Correlation-Id` documentado nas operacoes.
- Tags em portugues: `Autenticacao`, `Usuarios`, `Vendas`, `Cancelamentos` e `Saude`.
- OperationIds estaveis para integracao externa.
- Exemplos de request e response para criacao de venda.
- Erros padronizados documentados sem stack trace, segredo, SQL ou connection string.

Operacao rapida pelo Swagger:

1. Execute a API.
2. Abra `/swagger`.
3. Crie ou use um usuario.
4. Autentique em `POST /api/Auth`.
5. Autorize com `Bearer {token}`.
6. Execute `POST /api/sales`.
7. Execute `GET /api/sales`.
8. Execute os endpoints de consulta, atualizacao e cancelamento.

Comportamento por ambiente:

- Development: Swagger habilitado e detailed errors habilitados.
- UAT: Swagger habilitado para avaliacao e detailed errors limitados.
- Production: Swagger controlado por configuracao e desabilitado por padrao em uso real.

Guia detalhado:

```text
template/backend/docs/swagger.md
```

## Exemplo Completo com Curl

Criar usuario:

```powershell
curl.exe -X POST "http://localhost:8080/api/Users" ^
  -H "Content-Type: application/json" ^
  -d "{\"username\":\"salesuser\",\"password\":\"Sales@12345\",\"phone\":\"+5511999999999\",\"email\":\"salesuser@example.com\",\"status\":1,\"role\":1}"
```

Autenticar:

```powershell
curl.exe -X POST "http://localhost:8080/api/Auth" ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"salesuser@example.com\",\"password\":\"Sales@12345\"}"
```

Criar venda:

```powershell
curl.exe -X POST "http://localhost:8080/api/sales" ^
  -H "Authorization: Bearer {token}" ^
  -H "Content-Type: application/json" ^
  -d "{\"saleNumber\":\"SALE-2026-000001\",\"saleDate\":\"2026-05-26T12:00:00Z\",\"customerExternalId\":\"11111111-1111-1111-1111-111111111111\",\"customerName\":\"Cliente Exemplo\",\"branchExternalId\":\"22222222-2222-2222-2222-222222222222\",\"branchName\":\"Filial Centro\",\"items\":[{\"productExternalId\":\"33333333-3333-3333-3333-333333333333\",\"productName\":\"Produto Exemplo\",\"quantity\":10,\"unitPrice\":100}]}"
```

Resultado esperado da venda:

```text
quantity: 10
unitPrice: 100
discountPercentage: 20
totalAmount: 800
```

Listar venda:

```powershell
curl.exe -X GET "http://localhost:8080/api/sales?saleNumber=SALE-2026-000001" ^
  -H "Authorization: Bearer {token}"
```

Cancelar venda:

```powershell
curl.exe -X PATCH "http://localhost:8080/api/sales/{saleId}/cancel" ^
  -H "Authorization: Bearer {token}"
```

## Exemplo Completo com PowerShell

```powershell
$email = "sales.$([DateTimeOffset]::UtcNow.ToUnixTimeSeconds())@example.com"

$userBody = @{
  username = "salesuser"
  password = "Sales@12345"
  phone = "+5511999999999"
  email = $email
  status = 1
  role = 1
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri http://localhost:8080/api/Users -ContentType "application/json" -Body $userBody

$authBody = @{
  email = $email
  password = "Sales@12345"
} | ConvertTo-Json

$auth = Invoke-RestMethod -Method Post -Uri http://localhost:8080/api/Auth -ContentType "application/json" -Body $authBody
$headers = @{ Authorization = "Bearer $($auth.data.token)" }

$saleNumber = "SALE-$([DateTimeOffset]::UtcNow.ToUnixTimeSeconds())"
$saleBody = @{
  saleNumber = $saleNumber
  saleDate = "2026-05-26T12:00:00Z"
  customerExternalId = "11111111-1111-1111-1111-111111111111"
  customerName = "Cliente Exemplo"
  branchExternalId = "22222222-2222-2222-2222-222222222222"
  branchName = "Filial Centro"
  items = @(@{
    productExternalId = "33333333-3333-3333-3333-333333333333"
    productName = "Produto Exemplo"
    quantity = 10
    unitPrice = 100
  })
} | ConvertTo-Json -Depth 5

$sale = Invoke-RestMethod -Method Post -Uri http://localhost:8080/api/sales -Headers $headers -ContentType "application/json" -Body $saleBody
Invoke-RestMethod -Method Get -Uri "http://localhost:8080/api/sales?saleNumber=$saleNumber" -Headers $headers
Invoke-RestMethod -Method Patch -Uri "http://localhost:8080/api/sales/$($sale.data.id)/cancel" -Headers $headers
```

## Migrations

Criar nova migration:

```powershell
dotnet ef migrations add NomeDaMigration --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi --output-dir Migrations
```

Aplicar migrations:

```powershell
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

A migration de Sales criada nesta entrega:

```text
20260526164733_AddSales
```

## Eventos de Vendas

Eventos registrados em log:

```text
SaleCreated
SaleModified
SaleCancelled
ItemCancelled
```

Eles sao logs estruturados na aplicacao. Nao ha publicacao real em broker nesta entrega.

## Seguranca

Implementado:

- JWT Bearer.
- Validacao de issuer.
- Validacao de audience.
- Validacao de lifetime.
- Validacao de signing key.
- Rejeicao de secret JWT fraco em Production.
- Policies para Sales.
- CORS por ambiente.
- HSTS/HTTPS por configuracao.
- Security headers basicos.
- CorrelationId via `X-Correlation-Id`.
- Global exception middleware.

## Commits da Implementacao

Principais commits da entrega:

```text
feature: preparar develop e atualizar pacotes seguros
feature: separar ambientes development uat e production
feature: adicionar estabilidade seguranca e health checks
feature: implementar dominio de vendas e regras de desconto
feature: adicionar persistencia de vendas com postgresql
feature: implementar casos de uso de vendas
feature: expor endpoints de vendas
feature: registrar eventos de vendas em log
feature: documentar api de vendas no swagger
feature: adicionar testes unitarios de vendas
feature: documentar execucao ambientes e validacao do projeto
feature: ajustar validacao final da entrega
feature: ajustar smoke test de autenticacao e vendas
feature: atualizar documentacao oficial da prova
feature: melhorar configuracao openapi do swagger
feature: documentar endpoints de vendas no swagger
feature: documentar autenticacao e seguranca no swagger
```

## Documentacao Complementar

Pasta `.doc`:

```text
.doc/overview.md
.doc/tech-stack.md
.doc/frameworks.md
.doc/general-api.md
.doc/sales-api.md
.doc/project-structure.md
```

Pasta backend:

```text
template/backend/docs/architecture.md
template/backend/docs/environments.md
template/backend/docs/security.md
template/backend/docs/swagger.md
```

## Checklist Para Avaliacao

Antes de enviar o link do GitHub, valide:

```powershell
cd template/backend
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release
docker compose up --build
```

Em outro terminal:

```powershell
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
curl.exe -i http://localhost:8080/health/live
curl.exe -i http://localhost:8080/health/ready
```

Depois abra:

```text
http://localhost:8080/swagger
```

## Observacoes

- O projeto esta preparado para avaliacao local via Docker.
- O `appsettings.Production.json` nao contem segredo real.
- O banco padrao local usa usuario e senha apenas para desenvolvimento.
- Para reiniciar a base local do zero, use `docker compose down -v` e aplique migrations novamente.
