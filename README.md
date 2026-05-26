# Developer Evaluation Project

`READ CAREFULLY`

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)

<!-- 
## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)
-->

## Project Structure
This section describes the overall structure and organization of the project files and directories. 

See [Project Structure](/.doc/project-structure.md)

## Backend Implemented

The backend implementation is located at:

```text
template/backend
```

The existing template was evolved without rewriting the solution. Existing Auth and Users flows were preserved, and a complete Sales API was added following the project structure already present in the repository.

### Implemented Architecture

```text
template/backend/src
├── Ambev.DeveloperEvaluation.WebApi
├── Ambev.DeveloperEvaluation.Application
├── Ambev.DeveloperEvaluation.Domain
├── Ambev.DeveloperEvaluation.ORM
├── Ambev.DeveloperEvaluation.Common
└── Ambev.DeveloperEvaluation.IoC
```

Responsibilities:

- `WebApi`: HTTP controllers, request models, validation, middleware, Swagger and API responses.
- `Application`: MediatR commands, handlers, validators and result models.
- `Domain`: Sales aggregate, entities, business rules, domain events, exceptions and repository contracts.
- `ORM`: Entity Framework Core, PostgreSQL mappings, migrations and repository implementations.
- `Common`: shared security, validation, logging and health check infrastructure.
- `IoC`: dependency injection registration.

### Sales API

Implemented endpoints:

```http
POST   /api/sales
GET    /api/sales
GET    /api/sales/{id}
PUT    /api/sales/{id}
DELETE /api/sales/{id}
PATCH  /api/sales/{id}/cancel
PATCH  /api/sales/{id}/items/{itemId}/cancel
```

The list endpoint supports:

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

Sales endpoints require JWT authentication.

### Sales Data

The Sales API stores:

- Sale number
- Sale date
- Customer external identity and customer name
- Branch external identity and branch name
- Total sale amount
- Products
- Quantities
- Unit prices
- Discounts
- Total amount for each item
- Cancelled / Not Cancelled status

### External Identities

The implementation follows the External Identities pattern with denormalized descriptions:

```text
CustomerExternalId
CustomerName
BranchExternalId
BranchName
ProductExternalId
ProductName
```

No real Customer, Branch or Product domains were created.

### Business Rules

Business rules are implemented in the Domain layer, mainly in `Sale`, `SaleItem` and `SaleDiscountPolicy`.

```text
Quantity 1 to 3: 0% discount
Quantity 4 to 9: 10% discount
Quantity 10 to 20: 20% discount
Quantity above 20: invalid operation
Cancelled items do not compose the sale total
Cancelled sales cannot receive item changes
The sale aggregate recalculates totals
```

### Events

The following domain events were implemented:

```text
SaleCreated
SaleModified
SaleCancelled
ItemCancelled
```

Events are persisted in the `OutboxMessages` table in the same transaction as the sale change. The Worker can publish pending Outbox messages to Azure Service Bus when configured. In local simulation, the Worker uses `LogOnlyEventBusPublisher`.

### Outbox Tables

The backend includes the base Outbox structure:

```text
OutboxMessages
OutboxMessageAttempts
OutboxAdminActions
ProcessedIntegrationEvents
```

## How To Configure

Go to the backend folder:

```powershell
cd template/backend
```

The local PostgreSQL connection string is configured in:

```text
src/Ambev.DeveloperEvaluation.WebApi/appsettings.json
```

Current local value:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
  }
}
```

For local testing, Azure Key Vault is not required. For real hosted environments, sensitive values should be provided through environment variables, Azure App Settings, GitHub Secrets or Azure Key Vault.

Main configuration keys:

```text
ConnectionStrings__DefaultConnection
Jwt__SecretKey
Jwt__Issuer
Jwt__Audience
```

## How To Run

From `template/backend`, start PostgreSQL:

```powershell
docker compose up -d ambev.developerevaluation.database
```

Restore packages:

```powershell
dotnet restore
```

Build:

```powershell
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release --no-restore
```

Apply migrations:

```powershell
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Run the API:

```powershell
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Run the Outbox Worker:

```powershell
dotnet run --project src/Ambev.DeveloperEvaluation.Worker
```

Run with Docker Compose development profile:

```powershell
docker compose -f docker-compose.development.yml up --build
```

Swagger:

```text
http://localhost:5119/swagger
https://localhost:7181/swagger
```

Health checks:

```text
http://localhost:5119/health/live
http://localhost:5119/health/ready
```

## How To Test

Run all tests:

```powershell
dotnet test Ambev.DeveloperEvaluation.sln --configuration Release
```

Recommended manual flow:

```text
1. Start PostgreSQL.
2. Apply migrations.
3. Run the WebApi.
4. Create a user with POST /api/Users.
5. Authenticate with POST /api/Auth.
6. Copy the returned JWT token.
7. Click Authorize in Swagger.
8. Use the JWT token.
9. Test /api/sales endpoints.
```

### Create Sale Example

```json
{
  "saleNumber": "SALE-0001",
  "saleDate": "2026-05-26T12:00:00Z",
  "customerExternalId": "11111111-1111-1111-1111-111111111111",
  "customerName": "Example Customer",
  "branchExternalId": "22222222-2222-2222-2222-222222222222",
  "branchName": "Main Branch",
  "items": [
    {
      "productExternalId": "33333333-3333-3333-3333-333333333333",
      "productName": "Product A",
      "quantity": 4,
      "unitPrice": 10.00
    },
    {
      "productExternalId": "44444444-4444-4444-4444-444444444444",
      "productName": "Product B",
      "quantity": 10,
      "unitPrice": 20.00
    }
  ]
}
```

Expected financial result:

```text
Product A: 4 x 10.00 = 40.00, 10% discount, total 36.00
Product B: 10 x 20.00 = 200.00, 20% discount, total 160.00
Sale total: 196.00
```

## Database

Added migrations:

```text
AddSales
AddOutbox
```

Main tables:

```text
Users
Sales
SaleItems
OutboxMessages
OutboxMessageAttempts
OutboxAdminActions
ProcessedIntegrationEvents
```

## Redis Cache

Redis is used as read cache for:

```text
GET /api/sales/{id}
GET /api/sales
```

Cache strategy:

- Cache-aside.
- PostgreSQL remains the source of truth.
- Sales mutations invalidate detail and list cache.
- Redis failures do not block API responses.

Local Redis connection:

```text
ConnectionStrings__Redis=localhost:6379,password=ev@luAt10n
```

## Outbox Administration

Protected endpoints:

```http
GET    /api/admin/outbox/messages
GET    /api/admin/outbox/messages/{id}
POST   /api/admin/outbox/messages/{id}/retry
POST   /api/admin/outbox/messages/{id}/deadletter
POST   /api/admin/outbox/messages/{id}/reset
POST   /api/admin/outbox/retry-failed
POST   /api/admin/outbox/retry-deadlettered
GET    /api/admin/outbox/stats
```

Requirements:

- JWT token.
- `Queue.Admin` policy.
- User role `Admin`.

## Worker And Service Bus

Worker project:

```text
template/backend/src/Ambev.DeveloperEvaluation.Worker
```

Behavior:

- Reads pending Outbox messages.
- Locks messages before processing.
- Publishes to Azure Service Bus when `ServiceBus:Enabled=true` and connection string is configured.
- Uses LogOnly publisher when Service Bus is disabled.
- Marks messages as Published, Failed or DeadLettered.
- Does not crash on transient publish failures.

## Azure Functions

Functions project:

```text
template/backend/src/Ambev.DeveloperEvaluation.Functions
```

Implemented Functions:

```text
SaleCreatedFunction
SaleModifiedFunction
SaleCancelledFunction
ItemCancelledFunction
FunctionHealthCheck
```

The Functions consume Service Bus messages and use `ProcessedIntegrationEvents` to avoid duplicated processing by `EventId` and consumer name.

## Security

Implemented:

- JWT authentication.
- Issuer and audience validation when configured.
- Authorization policies for Sales and Queue Admin.
- Global exception middleware.
- Correlation id middleware.
- Security headers.
- Rate limiting.
- Startup configuration validation in Production.

## Health Checks

Endpoints:

```text
/health/live
/health/ready
/health
```

Readiness checks:

- PostgreSQL.
- Redis when cache is enabled and configured.

## Documentation

Additional documentation:

```text
docs/architecture.md
docs/security.md
docs/events.md
docs/queue-reprocessing.md
docs/observability.md
docs/key-vault.md
docs/deployment.md
docs/swagger.md
```

## CI/CD

Workflows:

```text
.github/workflows/ci.yml
.github/workflows/deploy-develop.yml
.github/workflows/deploy-uat.yml
.github/workflows/deploy-production-blue-green.yml
```

Deployment workflows are simulation-ready and include build/test/publish steps. Real Azure deployment requires OIDC/App Service/Function App secrets to be configured in GitHub environments.

## Automated Tests Added

Sales unit tests cover:

```text
Quantity 1 applies 0% discount
Quantity 3 applies 0% discount
Quantity 4 applies 10% discount
Quantity 9 applies 10% discount
Quantity 10 applies 20% discount
Quantity 20 applies 20% discount
Quantity 21 throws BusinessRuleException
Cancelled item does not compose sale total
Cancelled sale cannot be modified
Sale total is recalculated after update
SaleCreatedEvent is raised
SaleModifiedEvent is raised
SaleCancelledEvent is raised
ItemCancelledEvent is raised
```

## Notes

- `DELETE /api/sales/{id}` physically removes the sale.
- Sale cancellation is handled by `PATCH /api/sales/{id}/cancel`.
- The template includes Integration and Functional test projects, but they currently do not contain discovered tests.
- The build reports an existing known high severity vulnerability warning for `AutoMapper 13.0.1`.
