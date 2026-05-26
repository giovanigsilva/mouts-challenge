namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Dados completos de uma venda retornados pela API.
/// </summary>
public class SaleResult
{
    /// <summary>
    /// Identificador interno da venda.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Numero unico da venda.
    /// </summary>
    /// <example>SALE-2026-000001</example>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora em que a venda foi realizada.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Identificador externo do cliente.
    /// </summary>
    public Guid CustomerExternalId { get; set; }

    /// <summary>
    /// Nome do cliente salvo como snapshot no momento da venda.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Identificador externo da filial.
    /// </summary>
    public Guid BranchExternalId { get; set; }

    /// <summary>
    /// Nome da filial salvo como snapshot no momento da venda.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Valor total da venda apos descontos e sem itens cancelados.
    /// </summary>
    /// <example>380.00</example>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indica se a venda foi cancelada.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Data de criacao do registro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da ultima atualizacao do registro.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Itens da venda com descontos e totais calculados.
    /// </summary>
    public IEnumerable<SaleItemResult> Items { get; set; } = [];
}
