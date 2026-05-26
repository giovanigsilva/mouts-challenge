using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ambev.DeveloperEvaluation.WebApi.Swagger.Filters;

public sealed class SwaggerTagDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags =
        [
            new OpenApiTag { Name = "Autenticacao", Description = "Endpoints para autenticacao e emissao de token JWT." },
            new OpenApiTag { Name = "Usuarios", Description = "Endpoints de gerenciamento basico de usuarios preservados do template." },
            new OpenApiTag { Name = "Vendas", Description = "Criacao, consulta, listagem, atualizacao e remocao de vendas." },
            new OpenApiTag { Name = "Cancelamentos", Description = "Cancelamento comercial de venda e cancelamento de item." },
            new OpenApiTag { Name = "Saude", Description = "Endpoints de diagnostico de disponibilidade e prontidao da API." }
        ];
    }
}
