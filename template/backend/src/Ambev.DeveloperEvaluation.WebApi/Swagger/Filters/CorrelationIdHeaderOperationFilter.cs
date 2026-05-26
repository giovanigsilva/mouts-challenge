using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ambev.DeveloperEvaluation.WebApi.Swagger.Filters;

public sealed class CorrelationIdHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];
        if (operation.Parameters.Any(parameter => parameter.Name == "X-Correlation-Id"))
            return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Correlation-Id",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Identificador de correlacao usado para rastrear a requisicao nos logs. Se nao for informado, a API gera um automaticamente.",
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "uuid",
                Example = new Microsoft.OpenApi.Any.OpenApiString("a94fb91b-63e4-46c6-bd3d-82a89f7e1a4d")
            }
        });
    }
}
