# Deployment

Environments:

```text
develop -> ASPNETCORE_ENVIRONMENT=Development
uat -> ASPNETCORE_ENVIRONMENT=Uat
main -> ASPNETCORE_ENVIRONMENT=Production
```

Deploy units:

- WebApi to Azure App Service.
- Worker to App Service container or worker container.
- Functions to Azure Function App.

Production blue-green simulation:

1. Deploy to green/staging slot.
2. Run `/health/ready`.
3. Validate startup configuration.
4. Run smoke tests.
5. Swap green to production.
6. Run production smoke test.
7. Rollback with reverse slot swap if smoke fails.

Migrations:

- Development: can run `dotnet ef database update`.
- UAT: can apply non-destructive migrations automatically.
- Production: generate idempotent SQL script and require manual approval.

Command:

```powershell
dotnet ef migrations script --idempotent --project template/backend/src/Ambev.DeveloperEvaluation.ORM --startup-project template/backend/src/Ambev.DeveloperEvaluation.WebApi
```

