using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Common.Metrics;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.IoC.Caching;
using Ambev.DeveloperEvaluation.IoC.Observability;
using Ambev.DeveloperEvaluation.IoC.Options;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DefaultContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
            )
        );

        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        AddDeveloperStoreCache(builder.Services, builder.Configuration);
        AddDeveloperStoreMetrics(builder.Services, builder.Configuration);
    }

    private static IServiceCollection AddDeveloperStoreCache(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheOptions>(configuration.GetSection("Cache"));

        if (configuration.GetValue<bool>("Cache:Enabled"))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "developerstore:";
            });

            services.AddScoped<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddScoped<ICacheService, NoOpCacheService>();
        }

        services.AddScoped<ISalesCacheInvalidator, SalesCacheInvalidator>();

        return services;
    }

    private static IServiceCollection AddDeveloperStoreMetrics(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MetricsOptions>(configuration.GetSection("Metrics"));

        if (!configuration.GetValue<bool>("Metrics:Enabled"))
        {
            services.AddSingleton<IApplicationMetrics, NoOpApplicationMetrics>();
            return services;
        }

        var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "developerstore-sales-api";
        var serviceVersion = configuration["OpenTelemetry:ServiceVersion"] ?? "1.0.0";

        services.AddSingleton<IApplicationMetrics, DeveloperStoreMetrics>();
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName, serviceVersion: serviceVersion))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(DeveloperStoreMetrics.MeterName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (configuration.GetValue<bool>("Metrics:EnablePrometheusEndpoint"))
                    metrics.AddPrometheusExporter();
            });

        return services;
    }
}
