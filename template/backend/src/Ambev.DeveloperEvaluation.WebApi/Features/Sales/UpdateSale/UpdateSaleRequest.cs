using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Dados necessarios para atualizar uma venda existente.
/// </summary>
public class UpdateSaleRequest
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
    /// Identificador externo do cliente.
    /// </summary>
    public Guid CustomerExternalId { get; set; }

    /// <summary>
    /// Nome do cliente denormalizado.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Identificador externo da filial.
    /// </summary>
    public Guid BranchExternalId { get; set; }

    /// <summary>
    /// Nome da filial denormalizado.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Nova colecao de itens da venda. A atualizacao substitui os itens atuais.
    /// </summary>
    public IEnumerable<SaleItemRequest> Items { get; set; } = [];
}
