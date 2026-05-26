# Security

Implemented controls:

- JWT Bearer authentication.
- Issuer and audience validation when configured.
- Authorization policies:
  - `Sales.Read`
  - `Sales.Write`
  - `Sales.Cancel`
  - `Sales.Delete`
  - `Sales.Admin`
  - `Queue.Admin`
  - `Observability.Read`
- Sales endpoints require authorization.
- Outbox admin endpoints require `Queue.Admin`.
- Global exception middleware.
- Correlation id middleware.
- Security headers:
  - `X-Content-Type-Options`
  - `X-Frame-Options`
  - `Referrer-Policy`
  - `X-XSS-Protection`
- Rate limiting.
- Request DTOs are separated from domain entities.
- Sensitive payload is not returned by Outbox admin endpoints.

Secrets:

- Do not commit real production secrets.
- Use environment variables, GitHub Secrets, Azure App Settings or Azure Key Vault.
- Production startup validation checks required configuration.

Main keys:

```text
ConnectionStrings__DefaultConnection
ConnectionStrings__Redis
Jwt__SecretKey
Jwt__Issuer
Jwt__Audience
ServiceBus__ConnectionString
ServiceBus__FullyQualifiedNamespace
```

