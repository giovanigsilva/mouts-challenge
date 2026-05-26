namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Item de venda retornado pela API.
/// </summary>
public class SaleItemResult
{
    /// <summary>
    /// Identificador interno do item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador externo do produto.
    /// </summary>
    public Guid ProductExternalId { get; set; }

    /// <summary>
    /// Nome do produto salvo como snapshot no momento da venda.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade vendida do produto.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Preco unitario do produto.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Percentual de desconto aplicado automaticamente.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Valor monetario do desconto aplicado ao item.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Valor total do item apos desconto.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indica se o item foi cancelado.
    /// </summary>
    public bool IsCancelled { get; set; }
}
