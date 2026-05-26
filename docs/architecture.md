# Architecture

The backend is implemented under `template/backend` using layered Clean Architecture with DDD and MediatR.

```text
WebApi -> Application -> Domain
WebApi -> IoC -> ORM
Worker -> Application/Domain/ORM
Functions -> Application/Domain/ORM
```

Core decisions:

- `Sale` is the aggregate root.
- Business rules are in Domain.
- Controllers, handlers, repositories, workers and functions do not own business rules.
- PostgreSQL is the source of truth.
- Redis is a read cache only.
- Outbox stores integration messages in the same persistence boundary as Sales.
- Worker publishes Outbox messages after commit.
- Functions consume integration events and apply idempotency by `EventId`.

Main projects:

- `Ambev.DeveloperEvaluation.WebApi`: HTTP API, Swagger, security, health checks and admin endpoints.
- `Ambev.DeveloperEvaluation.Application`: use cases, cache abstraction and integration event contracts.
- `Ambev.DeveloperEvaluation.Domain`: entities, aggregate, policies, events, exceptions and repository contracts.
- `Ambev.DeveloperEvaluation.ORM`: EF Core mappings, migrations, repositories and Redis cache implementation.
- `Ambev.DeveloperEvaluation.Worker`: resilient Outbox publisher.
- `Ambev.DeveloperEvaluation.Functions`: Azure Functions isolated consumers.

