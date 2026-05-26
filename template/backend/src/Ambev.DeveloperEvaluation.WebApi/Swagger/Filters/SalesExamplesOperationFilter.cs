using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ambev.DeveloperEvaluation.WebApi.Swagger.Filters;

public sealed class SalesExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var path = context.ApiDescription.RelativePath?.ToLowerInvariant() ?? string.Empty;
        var method = context.ApiDescription.HttpMethod?.ToUpperInvariant();

        if (path == "api/sales" && method == "POST")
            ApplyCreateSaleExamples(operation);

        if (path == "api/sales" && method == "GET")
            operation.Description += "\n\nExemplo de filtro: GET /api/sales?page=1&pageSize=20&isCancelled=false&fromDate=2026-01-01&toDate=2026-12-31";
    }

    private static void ApplyCreateSaleExamples(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content.TryGetValue("application/json", out var requestJson) == true)
            requestJson.Example = CreateSaleRequestExample();

        if (operation.Responses.TryGetValue("201", out var createdResponse)
            && createdResponse.Content.TryGetValue("application/json", out var responseJson))
            responseJson.Example = SaleResponseExample("Venda criada com sucesso.");
    }

    private static OpenApiObject CreateSaleRequestExample()
    {
        return new OpenApiObject
        {
            ["saleNumber"] = new OpenApiString("SALE-2026-000001"),
            ["saleDate"] = new OpenApiString("2026-05-26T14:30:00Z"),
            ["customerExternalId"] = new OpenApiString("5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1"),
            ["customerName"] = new OpenApiString("Joao da Silva"),
            ["branchExternalId"] = new OpenApiString("7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2"),
            ["branchName"] = new OpenApiString("Loja Centro - Sao Paulo"),
            ["items"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["productExternalId"] = new OpenApiString("33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1"),
                    ["productName"] = new OpenApiString("Camiseta DeveloperStore"),
                    ["quantity"] = new OpenApiInteger(4),
                    ["unitPrice"] = new OpenApiDouble(50.00)
                },
                new OpenApiObject
                {
                    ["productExternalId"] = new OpenApiString("e7cb8e84-2c77-4020-bf2a-74cfce2b67cb"),
                    ["productName"] = new OpenApiString("Caneca DeveloperStore"),
                    ["quantity"] = new OpenApiInteger(10),
                    ["unitPrice"] = new OpenApiDouble(25.00)
                }
            }
        };
    }

    private static OpenApiObject SaleResponseExample(string message)
    {
        return new OpenApiObject
        {
            ["success"] = new OpenApiBoolean(true),
            ["message"] = new OpenApiString(message),
            ["errors"] = new OpenApiArray(),
            ["data"] = new OpenApiObject
            {
                ["id"] = new OpenApiString("8d7e1d6b-0cc1-44fd-aec4-1efc93968fa1"),
                ["saleNumber"] = new OpenApiString("SALE-2026-000001"),
                ["saleDate"] = new OpenApiString("2026-05-26T14:30:00Z"),
                ["customerExternalId"] = new OpenApiString("5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1"),
                ["customerName"] = new OpenApiString("Joao da Silva"),
                ["branchExternalId"] = new OpenApiString("7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2"),
                ["branchName"] = new OpenApiString("Loja Centro - Sao Paulo"),
                ["totalAmount"] = new OpenApiDouble(380.00),
                ["isCancelled"] = new OpenApiBoolean(false),
                ["createdAt"] = new OpenApiString("2026-05-26T14:31:00Z"),
                ["updatedAt"] = new OpenApiNull(),
                ["items"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiString("b1ac9f9a-40d7-4a7c-b8e7-8e513177f9dc"),
                        ["productExternalId"] = new OpenApiString("33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1"),
                        ["productName"] = new OpenApiString("Camiseta DeveloperStore"),
                        ["quantity"] = new OpenApiInteger(4),
                        ["unitPrice"] = new OpenApiDouble(50.00),
                        ["discountPercentage"] = new OpenApiDouble(10),
                        ["discountAmount"] = new OpenApiDouble(20.00),
                        ["totalAmount"] = new OpenApiDouble(180.00),
                        ["isCancelled"] = new OpenApiBoolean(false)
                    },
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiString("bc8d7891-1e6d-4f3d-9020-7a68e9c0c887"),
                        ["productExternalId"] = new OpenApiString("e7cb8e84-2c77-4020-bf2a-74cfce2b67cb"),
                        ["productName"] = new OpenApiString("Caneca DeveloperStore"),
                        ["quantity"] = new OpenApiInteger(10),
                        ["unitPrice"] = new OpenApiDouble(25.00),
                        ["discountPercentage"] = new OpenApiDouble(20),
                        ["discountAmount"] = new OpenApiDouble(50.00),
                        ["totalAmount"] = new OpenApiDouble(200.00),
                        ["isCancelled"] = new OpenApiBoolean(false)
                    }
                }
            }
        };
    }
}
