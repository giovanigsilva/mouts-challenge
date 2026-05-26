using Ambev.DeveloperEvaluation.Application.IntegrationEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Worker;
using Ambev.DeveloperEvaluation.Worker.Publishers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<DefaultContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        migration => migration.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")));

builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

var serviceBusEnabled = builder.Configuration.GetValue<bool>("ServiceBus:Enabled");
var serviceBusConnectionString = builder.Configuration["ServiceBus:ConnectionString"];

if (serviceBusEnabled && !string.IsNullOrWhiteSpace(serviceBusConnectionString))
{
    builder.Services.AddSingleton<IEventBusPublisher, AzureServiceBusEventPublisher>();
}
else
{
    builder.Services.AddSingleton<IEventBusPublisher, LogOnlyEventBusPublisher>();
}

builder.Services.AddHostedService<OutboxPublisherWorker>();

var host = builder.Build();
host.Run();
