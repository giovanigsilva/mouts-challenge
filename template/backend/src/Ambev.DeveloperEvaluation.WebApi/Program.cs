using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Configuration;
using Ambev.DeveloperEvaluation.WebApi.HealthChecks;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using Ambev.DeveloperEvaluation.WebApi.Swagger.Filters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            StartupConfigurationValidator.Validate(builder);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("PostgreSQL", tags: ["readiness"]);

            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = builder.Configuration["Swagger:Title"] ?? "DeveloperStore Sales API",
                    Version = builder.Configuration["Swagger:Version"] ?? "v1",
                    Description = BuildSwaggerDescription(builder.Configuration),
                    Contact = new OpenApiContact
                    {
                        Name = "DeveloperStore Team",
                        Email = "support@example.com"
                    }
                });
                options.AddServer(new OpenApiServer { Url = "http://localhost:8080", Description = "Development local via Docker" });
                options.AddServer(new OpenApiServer { Url = "http://localhost:5119", Description = "Development local via dotnet run" });
                options.AddServer(new OpenApiServer { Url = "https://uat-developerstore.example.com", Description = "UAT documental de exemplo" });
                options.AddServer(new OpenApiServer { Url = "https://developerstore.example.com", Description = "Production documental de exemplo" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Informe o token JWT no formato Bearer. Exemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);

                options.OperationFilter<AuthorizeOperationFilter>();
                options.OperationFilter<CorrelationIdHeaderOperationFilter>();
                options.OperationFilter<DefaultErrorResponsesOperationFilter>();
                options.OperationFilter<SalesExamplesOperationFilter>();
                options.DocumentFilter<SwaggerTagDocumentFilter>();
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy =>
                {
                    var origins = builder.Configuration.GetSection("Security:AllowedOrigins").Get<string[]>() ?? [];
                    if (origins.Length == 0)
                        policy.AllowAnyHeader().AllowAnyMethod();
                    else
                        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Sales.Read", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("Sales.Write", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("Sales.Cancel", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("Sales.Delete", policy => policy.RequireAuthenticatedUser());
            });

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(_ => { }, typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var app = builder.Build();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (builder.Configuration.GetValue<bool>("Swagger:Enabled"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (builder.Configuration.GetValue<bool>("Security:EnableHsts") && !app.Environment.IsDevelopment())
                app.UseHsts();

            if (builder.Configuration.GetValue<bool>("Security:EnableHttpsRedirection"))
                app.UseHttpsRedirection();

            if (builder.Configuration.GetValue<bool>("Security:EnableSecurityHeaders"))
                app.UseMiddleware<SecurityHeadersMiddleware>();

            app.UseCors("DefaultCors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static string BuildSwaggerDescription(IConfiguration configuration)
    {
        var configuredDescription = configuration["Swagger:Description"] ?? "API de vendas do DeveloperStore";
        return $"""
        {configuredDescription}

        API de vendas do DeveloperStore implementada em .NET 8 com Clean Architecture, DDD, CQRS com MediatR, Entity Framework Core, PostgreSQL, FluentValidation, JWT Bearer, Serilog e Swagger/OpenAPI.

        Objetivo: permitir o gerenciamento completo de vendas, incluindo criacao, consulta, listagem, atualizacao, remocao, cancelamento de venda e cancelamento de item.

        Sales usa External Identities: a API nao consulta dominios reais de Customer, Branch ou Product. A venda armazena externalId e nome denormalizado de cliente, filial e produto como snapshot no momento da venda.

        Regras de desconto: 1 a 3 unidades do mesmo produto nao recebem desconto; 4 a 9 unidades recebem 10%; 10 a 20 unidades recebem 20%; acima de 20 unidades e uma operacao invalida. Produto duplicado na mesma venda nao e permitido.

        Cancelamento: venda cancelada nao pode ser alterada. Item cancelado nao compoe o total financeiro da venda.

        Eventos: SaleCreated, SaleModified, SaleCancelled e ItemCancelled sao registrados no log da aplicacao. Nesta prova nao ha broker ou mensageria real.

        Autenticacao: endpoints de Sales exigem JWT Bearer. Use o endpoint de autenticacao, copie o token e clique em Authorize informando Bearer seguido do token.

        Padrao de resposta: a API usa ApiResponse, ApiResponseWithData<T> e PaginatedResponse<T>. Erros retornam success=false e correlationId quando tratados pelo middleware global.

        Ambientes: Development habilita Swagger e detailed errors; UAT habilita Swagger para avaliacao com detalhes limitados; Production mantem Swagger desabilitado por padrao e exige segredos por variaveis de ambiente.

        Health checks: /health/live indica processo vivo e /health/ready valida prontidao e PostgreSQL.

        X-Correlation-Id: header opcional para rastrear requisicoes. Se nao informado, a API gera um automaticamente.

        Como testar rapidamente: crie um usuario em /api/Users, autentique em /api/Auth, copie o token JWT, clique em Authorize, informe Bearer seguido do token, execute POST /api/sales, consulte GET /api/sales e teste os PATCH de cancelamento.
        """;
    }
}
