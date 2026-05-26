using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Common.Behaviors;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.HealthChecks;
using Ambev.DeveloperEvaluation.WebApi.Swagger;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Configuration;

public static class WebApiServiceExtensions
{
    public static WebApplicationBuilder AddWebApiServices(this WebApplicationBuilder builder)
    {
        builder.AddDefaultLogging();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.AddBasicHealthChecks();
        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("PostgreSQL", tags: ["readiness"]);
        builder.Services.AddDeveloperStoreSwagger(builder.Configuration);
        builder.Services.AddDeveloperStoreCors(builder.Configuration);
        builder.Services.AddDeveloperStoreDatabase(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddDeveloperStoreAuthorization();
        builder.RegisterDependencies();
        builder.Services.AddDeveloperStoreApplication();

        return builder;
    }

    private static IServiceCollection AddDeveloperStoreCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors", policy =>
            {
                var origins = configuration.GetSection("Security:AllowedOrigins").Get<string[]>() ?? [];
                if (origins.Length == 0)
                    policy.AllowAnyHeader().AllowAnyMethod();
                else
                    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }

    private static IServiceCollection AddDeveloperStoreDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DefaultContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
            )
        );

        return services;
    }

    private static IServiceCollection AddDeveloperStoreAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Sales.Read", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("Sales.Write", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("Sales.Cancel", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("Sales.Delete", policy => policy.RequireAuthenticatedUser());
        });

        return services;
    }

    private static IServiceCollection AddDeveloperStoreApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            );
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
