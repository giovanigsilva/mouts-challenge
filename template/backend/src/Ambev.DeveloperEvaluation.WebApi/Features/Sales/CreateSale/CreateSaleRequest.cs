using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Dados necessarios para criar uma venda.
/// </summary>
public class CreateSaleRequest
{
    /// <summary>
    /// Numero unico da venda.
    /// </summary>
    /// <example>SALE-2026-000001</example>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora em que a venda foi realizada.
    /// </summary>
    /// <example>2026-05-26T14:30:00Z</example>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Identificador externo do cliente no dominio de clientes.
    /// </summary>
    /// <example>5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1</example>
    public Guid CustomerExternalId { get; set; }

    /// <summary>
    /// Nome do cliente denormalizado no momento da venda.
    /// </summary>
    /// <example>Joao da Silva</example>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Identificador externo da filial onde a venda foi realizada.
    /// </summary>
    /// <example>7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2</example>
    public Guid BranchExternalId { get; set; }

    /// <summary>
    /// Nome da filial denormalizado no momento da venda.
    /// </summary>
    /// <example>Loja Centro - Sao Paulo</example>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Itens vendidos. Cada produto pode aparecer apenas uma vez na venda.
    /// </summary>
    public IEnumerable<SaleItemRequest> Items { get; set; } = [];
}
