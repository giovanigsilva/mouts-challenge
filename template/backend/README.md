# DeveloperStore Sales API

API do desafio DeveloperStore implementada em .NET 8 com Clean Architecture, DDD, CQRS com MediatR, Entity Framework Core, PostgreSQL, FluentValidation, JWT Bearer, Serilog, Swagger/OpenAPI em portugues, Docker, Seq, Datadog Agent e HashiCorp Vault local para segredos de Development.

Este README foi escrito para o avaliador clonar o repositorio e inicializar a API do zero.

## Sumario

- [O que esta implementado](#o-que-esta-implementado)
- [Pre-requisitos](#pre-requisitos)
- [Inicializacao rapida com Docker](#inicializacao-rapida-com-docker)
- [Migrations](#migrations)
- [Swagger](#swagger)
- [Fluxo de teste por curl](#fluxo-de-teste-por-curl)
- [Rodar sem container para a API](#rodar-sem-container-para-a-api)
- [Testes](#testes)
- [Ambientes](#ambientes)
- [Segredos e Vault local](#segredos-e-vault-local)
- [Observabilidade](#observabilidade)
- [Troubleshooting](#troubleshooting)

## O que esta implementado

- Auth com JWT.
- Users.
- CRUD completo de Sales.
- Cancelamento de venda.
- Cancelamento de item.
- Regras de desconto por quantidade no dominio.
- External Identities com snapshot denormalizado para cliente, filial e produto.
- Eventos de venda registrados em log: `SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled`.
- PostgreSQL com EF Core migrations no projeto `Ambev.DeveloperEvaluation.ORM`.
- Health checks: `/health/live`, `/health/ready`, `/health/logging`.
- Swagger em portugues.
- FluentValidation em requests, commands e queries.
- CorrelationId via header `X-Correlation-Id`.
- Logs estruturados com Serilog.
- Seq local em `http://localhost:5341`.
- Datadog Agent configurado via Docker.
- Vault local em modo dev para segredos de Development.
- Separacao de ambientes: `Development`, `Uat` e `Production`.

Fora do escopo desta prova: Redis real, Azure Service Bus real, Azure Functions reais, Azure Key Vault real e blue-green real.

## Pre-requisitos

Instale:

- Git.
- Docker Desktop com Docker Compose.
- .NET SDK 8.
- PowerShell.

Confira as versoes:

```powershell
git --version
docker --version
docker compose version
dotnet --version
```

Instale o Entity Framework CLI se ainda nao existir:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef --version
```

## Clone e pasta do backend

```powershell
git clone <URL_DO_REPOSITORIO>
cd mouts-challenge\template\backend
git checkout develop
```

Se o repositorio ja estiver clonado nesta maquina:

```powershell
cd C:\Users\Giovani\source\repos\mouts-challenge\template\backend
git checkout develop
```

## Inicializacao rapida com Docker

Este e o fluxo recomendado para avaliar o projeto.

### 1. Limpar execucoes antigas

Use este comando quando quiser iniciar realmente do zero. Ele remove containers e volumes locais do projeto, incluindo banco local e dados do Seq.

```powershell
docker compose down --volumes --remove-orphans
```

### 2. Subir infraestrutura local

```powershell
docker compose up -d ambev.developerevaluation.database vault seq
```

Servicos esperados:

- PostgreSQL: `localhost:5432`
- Vault dev: `http://localhost:8200`
- Seq: `http://localhost:5341`

### 3. Popular segredos no Vault local

PowerShell:

```powershell
$env:VAULT_ADDR="http://localhost:8200"
$env:VAULT_TOKEN="dev-root-token"
.\scripts\vault-init-dev.ps1
```

Linux/macOS:

```bash
export VAULT_ADDR=http://localhost:8200
export VAULT_TOKEN=dev-root-token
./scripts/vault-init-dev.sh
```

O script usa o Vault CLI se estiver instalado. Se nao estiver, ele executa o comando pelo container `developerstore-vault`.

### 4. Aplicar migrations

Com o PostgreSQL e o Vault rodando, aplique as migrations:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

### 5. Subir a WebApi

```powershell
docker compose up --build -d ambev.developerevaluation.webapi
```

### 6. Verificar containers

```powershell
docker compose ps
```

Devem aparecer pelo menos:

- `ambev_developer_evaluation_webapi`
- `ambev_developer_evaluation_database`
- `developerstore-vault`
- `developerstore_seq`

O Datadog Agent e opcional para a prova local. Para subi-lo tambem:

```powershell
docker compose up -d datadog-agent
```

## Migrations

Projeto de migrations:

```text
src/Ambev.DeveloperEvaluation.ORM
```

Startup project:

```text
src/Ambev.DeveloperEvaluation.WebApi
```

Comando:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Connection string local usada pelo host:

```text
Host=localhost;Port=5432;Database=developer_evaluation;Username=developerstore_app;Password=DevOnly_Pg_9fR!42sL#2026_Strong
```

Connection string usada dentro do Docker:

```text
Host=ambev.developerevaluation.database;Port=5432;Database=developer_evaluation;Username=developerstore_app;Password=DevOnly_Pg_9fR!42sL#2026_Strong
```

## Health checks

Com a API no Docker:

```powershell
curl.exe -i http://localhost:8080/health/live
curl.exe -i http://localhost:8080/health/ready
curl.exe -i http://localhost:8080/health/logging
```

Resultado esperado:

- `/health/live`: `200 OK`, processo vivo.
- `/health/ready`: `200 OK`, API pronta e PostgreSQL/configuracoes essenciais OK.
- `/health/logging`: `200 OK`, configuracao de logs sem expor segredo.

## Swagger

Com Docker:

```text
http://localhost:8080/swagger
```

Com `dotnet run` local:

```text
http://localhost:5119/swagger
```

No Swagger:

1. Crie um usuario em `POST /api/Users`.
2. Autentique em `POST /api/Auth`.
3. Copie o token retornado.
4. Clique em `Authorize`.
5. Informe `Bearer <TOKEN>`.
6. Teste os endpoints de Sales.

## Fluxo de teste por curl

Os exemplos abaixo usam PowerShell e `curl.exe`.

### 1. Criar usuario admin

```powershell
curl.exe -i -X POST http://localhost:8080/api/Users `
  -H "Content-Type: application/json" `
  -H "X-Correlation-Id: 11111111-1111-1111-1111-111111111111" `
  -d '{\"username\":\"Admin DeveloperStore\",\"password\":\"Senha@123456\",\"phone\":\"11999999999\",\"email\":\"admin@developerstore.com\",\"status\":1,\"role\":3}'
```

Valores confirmados nos enums do projeto:

- `status: 1` = `Active`
- `role: 3` = `Admin`

### 2. Autenticar

```powershell
curl.exe -i -X POST http://localhost:8080/api/Auth `
  -H "Content-Type: application/json" `
  -d '{\"email\":\"admin@developerstore.com\",\"password\":\"Senha@123456\"}'
```

Copie o token retornado em `data.token`.

```powershell
$TOKEN="<COLE_AQUI_O_TOKEN>"
```

### 3. Criar venda

```powershell
curl.exe -i -X POST http://localhost:8080/api/sales `
  -H "Content-Type: application/json" `
  -H "Authorization: Bearer $TOKEN" `
  -H "X-Correlation-Id: 22222222-2222-2222-2222-222222222222" `
  -d '{\"saleNumber\":\"SALE-2026-000001\",\"saleDate\":\"2026-05-26T14:30:00Z\",\"customerExternalId\":\"5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1\",\"customerName\":\"Joao da Silva\",\"branchExternalId\":\"7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2\",\"branchName\":\"Loja Centro - Sao Paulo\",\"items\":[{\"productExternalId\":\"33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1\",\"productName\":\"Camiseta DeveloperStore\",\"quantity\":4,\"unitPrice\":50.00},{\"productExternalId\":\"e7cb8e84-2c77-4020-bf2a-74cfce2b67cb\",\"productName\":\"Caneca DeveloperStore\",\"quantity\":10,\"unitPrice\":25.00}]}'
```

Calculo esperado:

- Camiseta: `4 * 50.00 = 200.00`, desconto 10%, total `180.00`.
- Caneca: `10 * 25.00 = 250.00`, desconto 20%, total `200.00`.
- Total da venda: `380.00`.

### 4. Listar vendas

```powershell
curl.exe -i "http://localhost:8080/api/sales?page=1&pageSize=20" `
  -H "Authorization: Bearer $TOKEN"
```

Com filtros:

```powershell
curl.exe -i "http://localhost:8080/api/sales?page=1&pageSize=20&isCancelled=false&fromDate=2026-01-01&toDate=2026-12-31" `
  -H "Authorization: Bearer $TOKEN"
```

### 5. Consultar venda por id

Substitua `<SALE_ID>` pelo `data.id` retornado na criacao.

```powershell
curl.exe -i http://localhost:8080/api/sales/<SALE_ID> `
  -H "Authorization: Bearer $TOKEN"
```

### 6. Atualizar venda

```powershell
curl.exe -i -X PUT http://localhost:8080/api/sales/<SALE_ID> `
  -H "Content-Type: application/json" `
  -H "Authorization: Bearer $TOKEN" `
  -d '{\"saleNumber\":\"SALE-2026-000001\",\"saleDate\":\"2026-05-26T15:00:00Z\",\"customerExternalId\":\"5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1\",\"customerName\":\"Joao da Silva\",\"branchExternalId\":\"7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2\",\"branchName\":\"Loja Centro - Sao Paulo\",\"items\":[{\"productExternalId\":\"33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1\",\"productName\":\"Camiseta DeveloperStore\",\"quantity\":9,\"unitPrice\":50.00}]}'
```

### 7. Cancelar item da venda

Substitua `<ITEM_ID>` pelo id do item retornado na venda.

```powershell
curl.exe -i -X PATCH http://localhost:8080/api/sales/<SALE_ID>/items/<ITEM_ID>/cancel `
  -H "Authorization: Bearer $TOKEN"
```

Item cancelado nao compoe o total financeiro da venda.

### 8. Cancelar venda

```powershell
curl.exe -i -X PATCH http://localhost:8080/api/sales/<SALE_ID>/cancel `
  -H "Authorization: Bearer $TOKEN"
```

Venda cancelada nao permite novas alteracoes de itens.

### 9. Remover venda

```powershell
curl.exe -i -X DELETE http://localhost:8080/api/sales/<SALE_ID> `
  -H "Authorization: Bearer $TOKEN"
```

`DELETE` remove fisicamente. Cancelamento comercial e feito em `PATCH /api/sales/{id}/cancel`.

## Regras de negocio de Sales

- 1 a 3 unidades do mesmo produto: 0% de desconto.
- 4 a 9 unidades do mesmo produto: 10% de desconto.
- 10 a 20 unidades do mesmo produto: 20% de desconto.
- Acima de 20 unidades do mesmo produto: operacao invalida.
- Produto duplicado na mesma venda nao e permitido.
- Item cancelado nao entra no total da venda.
- Venda cancelada nao permite alteracao de itens.
- Desconto manual nao e aceito no request.

## Rodar sem container para a API

Suba somente a infraestrutura:

```powershell
docker compose up -d ambev.developerevaluation.database vault seq
```

Popule o Vault:

```powershell
$env:VAULT_ADDR="http://localhost:8200"
$env:VAULT_TOKEN="dev-root-token"
.\scripts\vault-init-dev.ps1
```

Aplique migrations:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Rode a API:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Abra:

```text
http://localhost:5119/swagger
```

## Testes

Comandos:

```powershell
dotnet restore
dotnet build .\Ambev.DeveloperEvaluation.sln --configuration Release --no-restore
dotnet test .\Ambev.DeveloperEvaluation.sln --configuration Release --no-build
```

Estado atual do projeto:

- `tests/Ambev.DeveloperEvaluation.Unit` contem testes unitarios.
- `tests/Ambev.DeveloperEvaluation.Functional` e `tests/Ambev.DeveloperEvaluation.Integration` existem na solution, mas podem aparecer sem testes descobertos.

## Ambientes

### Development

Arquivo:

```text
src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json
```

Caracteristicas:

- Swagger habilitado.
- Detailed errors habilitados.
- Vault local opcional.
- Fallback controlado para appsettings/env vars.
- CORS local.
- Seq habilitado.

Rodar:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

### UAT

Arquivo:

```text
src/Ambev.DeveloperEvaluation.WebApi/appsettings.Uat.json
```

Rodar:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Uat"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

UAT usa valores fortes de simulacao para a prova. Em ambiente real, sobrescreva segredos por variaveis de ambiente.

### Production

Arquivo:

```text
src/Ambev.DeveloperEvaluation.WebApi/appsettings.Production.json
```

Production nao possui segredo real commitado. Para simular localmente, configure variaveis:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__DefaultConnection="Host=prod-postgres.internal;Port=5432;Database=developerstore;Username=prod_app;Password=Prod_Strong_9gT!41pL#2026"
$env:Jwt__SecretKey="Prod_JwtSecret_2026_pZ9!mQ4#vL8@rT2%DeveloperStore"
$env:Jwt__Issuer="DeveloperStore.Production"
$env:Jwt__Audience="DeveloperStore.Api"
$env:Security__AllowedOrigins__0="https://developerstore.example.com"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Se faltar segredo obrigatorio ou houver valor fraco/local, a API falha no startup de proposito.

## Segredos e Vault local

Vault local:

```text
http://localhost:8200
```

Token dev:

```text
dev-root-token
```

Caminho dos segredos:

```text
secret/developerstore/development
```

Valores gravados pelo script:

```text
Jwt__SecretKey=DevOnly_JwtSecret_2026_pZ9!mQ4#vL8@rT2%DeveloperStore
Jwt__Issuer=DeveloperStore.Development
Jwt__Audience=DeveloperStore.Api
ConnectionStrings__DefaultConnection=Host=ambev.developerevaluation.database;Port=5432;Database=developer_evaluation;Username=developerstore_app;Password=DevOnly_Pg_9fR!42sL#2026_Strong
```

Consultar o Vault via container:

```powershell
docker exec -e VAULT_ADDR=http://127.0.0.1:8200 -e VAULT_TOKEN=dev-root-token developerstore-vault vault kv get secret/developerstore/development
```

Estes valores sao somente para prova de conceito local.

## Observabilidade

Seq:

```text
http://localhost:5341
```

Datadog Agent:

```powershell
docker compose up -d datadog-agent
```

Para enviar logs reais ao Datadog, crie um `.env` baseado em `.env.example`:

```text
DD_API_KEY=coloque_sua_api_key_datadog_aqui
DD_SITE=datadoghq.com
DD_ENV=development
DD_SERVICE=developerstore-sales-api
DD_VERSION=1.0.0
```

Sem `DD_API_KEY` real, a API continua funcionando e Seq continua sendo o fallback local pesquisavel.

Pesquisas uteis no Seq:

```text
correlationId = '22222222-2222-2222-2222-222222222222'
requestPath like '%sales%'
saleNumber = 'SALE-2026-000001'
```

## Estrutura principal

```text
src/Ambev.DeveloperEvaluation.WebApi
src/Ambev.DeveloperEvaluation.Application
src/Ambev.DeveloperEvaluation.Domain
src/Ambev.DeveloperEvaluation.ORM
src/Ambev.DeveloperEvaluation.Common
src/Ambev.DeveloperEvaluation.IoC
tests/Ambev.DeveloperEvaluation.Unit
tests/Ambev.DeveloperEvaluation.Functional
tests/Ambev.DeveloperEvaluation.Integration
docs
scripts
```

## Documentacao complementar

- [Arquitetura](docs/architecture.md)
- [Ambientes](docs/environments.md)
- [Swagger](docs/swagger.md)
- [Seguranca](docs/security.md)
- [Segredos](docs/secrets.md)
- [Observabilidade](docs/observability.md)
- [Logs](docs/logging.md)
- [Resiliencia](docs/resilience.md)

## Troubleshooting

### PostgreSQL nao aceita usuario/senha

Provavel volume antigo. Reset:

```powershell
docker compose down --volumes --remove-orphans
docker compose up -d ambev.developerevaluation.database vault seq
$env:VAULT_ADDR="http://localhost:8200"
$env:VAULT_TOKEN="dev-root-token"
.\scripts\vault-init-dev.ps1
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
docker compose up --build -d ambev.developerevaluation.webapi
```

### Vault nao esta pronto

Rode novamente:

```powershell
.\scripts\vault-init-dev.ps1
```

O script tenta aguardar o Vault antes de gravar os segredos.

### Swagger nao abre

Confira se a API esta viva:

```powershell
docker logs --tail 100 ambev_developer_evaluation_webapi
curl.exe -i http://localhost:8080/health/live
```

### Health ready falha

Veja logs da API:

```powershell
docker logs --tail 200 ambev_developer_evaluation_webapi
```

Verifique se as migrations foram aplicadas:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

### Erro 401 em Sales

Crie usuario, autentique em `/api/Auth`, copie `data.token` e envie:

```text
Authorization: Bearer <TOKEN>
```

### Erro 403 em Sales

Use usuario com role `Admin` no cadastro:

```json
"role": 3
```

### Porta 8080 ou 5432 ocupada

Verifique containers existentes:

```powershell
docker ps
```

Pare a stack:

```powershell
docker compose down
```

## Links locais

- API Docker: `http://localhost:8080`
- Swagger Docker: `http://localhost:8080/swagger`
- API via `dotnet run`: `http://localhost:5119`
- Swagger via `dotnet run`: `http://localhost:5119/swagger`
- Seq: `http://localhost:5341`
- Vault: `http://localhost:8200`
- PostgreSQL: `localhost:5432`

