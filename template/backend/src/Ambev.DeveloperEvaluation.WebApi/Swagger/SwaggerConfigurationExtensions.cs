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
        var configuredDescription = configuration["Swagger:Description"] ?? "API de vendas do DeveloperStore";
        return $"""
        {configuredDescription}

        API de vendas do DeveloperStore implementada em .NET 8 com Clean Architecture, DDD, CQRS com MediatR, Entity Framework Core, PostgreSQL, FluentValidation, JWT Bearer, Serilog e Swagger/OpenAPI.

        Objetivo: permitir o gerenciamento completo de vendas, incluindo criacao, consulta, listagem, atualizacao, remocao, cancelamento de venda e cancelamento de item.

        Sales usa External Identities: a API nao consulta dominios reais de Customer, Branch ou Product. A venda armazena externalId e nome denormalizado de cliente, filial e produto como snapshot no momento da venda.

        Regras de desconto: 1 a 3 unidades do mesmo produto nao recebem desconto; 4 a 9 unidades recebem 10%; 10 a 20 unidades recebem 20%; acima de 20 unidades e uma operacao invalida. Produto duplicado na mesma venda nao e permitido.

        Cancelamento: venda cancelada nao pode ser alterada. Item cancelado nao compoe o total financeiro da venda.

        Eventos: SaleCreated, SaleModified, SaleCancelled e ItemCancelled sao registrados no log estruturado da aplicacao. Nesta prova nao ha broker ou mensageria real.

        Observabilidade: a aplicacao escreve logs JSON no console para coleta pelo Datadog Agent no Docker. Seq e o unico fallback local pesquisavel. Falhas de observabilidade nao devem derrubar requests de negocio.

        Autenticacao: endpoints de Sales exigem JWT Bearer. Use o endpoint de autenticacao, copie o token e clique em Authorize informando Bearer seguido do token.

        Padrao de resposta: a API usa ApiResponse, ApiResponseWithData<T> e PaginatedResponse<T>. Erros retornam success=false e correlationId quando tratados pelo middleware global.

        Segredos: em Development a API pode carregar segredos do HashiCorp Vault local em Docker quando Vault:Enabled=true. Se o Vault local estiver indisponivel em Development, existe fallback controlado para appsettings/env vars. Em UAT e Production, segredos devem vir de variaveis de ambiente ou do provedor seguro do ambiente hospedado. Production rejeita segredos fracos, placeholders e valores development-only.

        Ambientes: Development habilita Swagger e detailed errors; UAT habilita Swagger para avaliacao com detalhes limitados; Production mantem Swagger desabilitado por padrao e exige segredos por variaveis de ambiente.

        Health checks: /health/live indica processo vivo e /health/ready valida prontidao, PostgreSQL, configuracoes criticas de seguranca e Vault local quando habilitado em Development.

        X-Correlation-Id: header opcional para rastrear requisicoes. Se nao informado, a API gera um automaticamente. Logs incluem correlationId, traceId, usuario quando autenticado, metodo HTTP, path, status code e duracao.

        Como testar rapidamente: crie um usuario em /api/Users, autentique em /api/Auth, copie o token JWT, clique em Authorize, informe Bearer seguido do token, execute POST /api/sales, consulte GET /api/sales e teste os PATCH de cancelamento.
        """;
    }
}
