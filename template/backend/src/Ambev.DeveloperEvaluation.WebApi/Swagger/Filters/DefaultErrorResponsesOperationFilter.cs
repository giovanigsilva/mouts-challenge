using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ambev.DeveloperEvaluation.WebApi.Swagger.Filters;

public sealed class DefaultErrorResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
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
