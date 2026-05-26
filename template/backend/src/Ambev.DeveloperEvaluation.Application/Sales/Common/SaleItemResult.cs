namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public sealed class SaleItemResult
{
    public Guid Id { get; set; }
    public Guid ProductExternalId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
}
