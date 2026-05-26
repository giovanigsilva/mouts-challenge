namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// Item informado na criacao ou atualizacao de uma venda.
/// </summary>
public class SaleItemRequest
{
    /// <summary>
    /// Identificador externo do produto no dominio de produtos.
    /// </summary>
    /// <example>33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1</example>
    public Guid ProductExternalId { get; set; }

    /// <summary>
    /// Nome do produto denormalizado no momento da venda.
    /// </summary>
    /// <example>Camiseta DeveloperStore</example>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade vendida do produto. Deve estar entre 1 e 20.
    /// </summary>
    /// <example>4</example>
    public int Quantity { get; set; }

    /// <summary>
    /// Preco unitario do produto. Deve ser maior que zero.
    /// </summary>
    /// <example>50.00</example>
    public decimal UnitPrice { get; set; }
}
