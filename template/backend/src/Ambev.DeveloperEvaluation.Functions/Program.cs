using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<DefaultContext>(options =>
            options.UseNpgsql(
                context.Configuration.GetConnectionString("DefaultConnection"),
                migration => migration.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")));
    })
    .Build();

host.Run();
