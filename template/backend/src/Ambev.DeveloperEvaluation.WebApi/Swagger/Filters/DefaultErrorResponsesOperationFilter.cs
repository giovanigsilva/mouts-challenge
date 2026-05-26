using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ambev.DeveloperEvaluation.WebApi.Swagger.Filters;

public sealed class DefaultErrorResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        AddResponse(operation, "400", "Requisicao invalida, erro de validacao ou violacao de regra de negocio.");
        AddResponse(operation, "401", "Autenticacao ausente ou invalida.");
        AddResponse(operation, "403", "Usuario autenticado sem permissao suficiente para a operacao.");
        AddResponse(operation, "404", "Recurso nao encontrado.");
        AddResponse(operation, "408", "A requisicao excedeu o tempo limite de processamento.");
        AddResponse(operation, "409", "Conflito ao processar a operacao, como concorrencia de dados.");
        AddResponse(operation, "429", "Muitas requisicoes em um curto periodo. Tente novamente mais tarde.");
        AddResponse(operation, "499", "Requisicao cancelada pelo cliente antes da conclusao do processamento.");
        AddResponse(operation, "500", "Erro interno inesperado. Informe o correlationId ao suporte.");
    }

    private static void AddResponse(OpenApiOperation operation, string statusCode, string description)
    {
        if (operation.Responses.ContainsKey(statusCode))
            return;

        operation.Responses.Add(statusCode, new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new()
                {
                    Schema = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = nameof(ApiResponse)
                        }
                    }
                }
            }
        });
    }
}
