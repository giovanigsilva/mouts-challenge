using Ambev.DeveloperEvaluation.WebApi.Swagger.Filters;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.WebApi.Swagger;

public static class SwaggerConfigurationExtensions
{
    public static IServiceCollection AddDeveloperStoreSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = configuration["Swagger:Title"] ?? "DeveloperStore Sales API",
                Version = configuration["Swagger:Version"] ?? "v1",
                Description = BuildSwaggerDescription(configuration),
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

        return services;
    }

    private static string BuildSwaggerDescription(IConfiguration configuration)
    {
        return """
        API de vendas do DeveloperStore implementada em .NET 8 com Clean Architecture, DDD, CQRS/MediatR, EF Core, PostgreSQL, FluentValidation, JWT Bearer, Serilog e Swagger/OpenAPI.

        Permite criar, consultar, listar, atualizar, remover e cancelar vendas e itens.

        A venda usa External Identities: Customer, Branch e Product não são consultados diretamente. A API grava externalId e nome como snapshot no momento da venda.

        Regras principais:
        - 1 a 3 unidades: sem desconto
        - 4 a 9 unidades: 10%
        - 10 a 20 unidades: 20%
        - Acima de 20 unidades: inválido
        - Produto duplicado na mesma venda não é permitido
        - Venda cancelada não pode ser alterada
        - Item cancelado não compõe o total da venda

        Eventos registrados em log estruturado: SaleCreated, SaleModified, SaleCancelled e ItemCancelled.

        Endpoints de Sales exigem JWT Bearer. Crie um usuário em /api/Users, autentique em /api/Auth e use Authorize com: Bearer {token}.
        """;
    }
}
