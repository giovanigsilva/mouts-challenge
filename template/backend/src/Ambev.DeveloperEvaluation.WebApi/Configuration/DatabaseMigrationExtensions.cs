using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Configuration;

public static class DatabaseMigrationExtensions
{
    public static async Task<WebApplication> ApplyDatabaseMigrationsAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DefaultContext>>();

        logger.LogInformation("Verificando conectividade com o PostgreSQL antes de iniciar a API");

        if (!await context.Database.CanConnectAsync(cancellationToken))
            throw new InvalidOperationException("Nao foi possivel conectar ao PostgreSQL para validar/aplicar migrations.");

        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        if (pendingMigrations.Count == 0)
        {
            logger.LogInformation("Banco de dados atualizado. Nenhuma migration pendente encontrada");
            return app;
        }

        logger.LogInformation("Aplicando {PendingMigrationCount} migration(s) pendente(s) no PostgreSQL", pendingMigrations.Count);

        await context.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Migrations aplicadas com sucesso no PostgreSQL");

        return app;
    }
}
