namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public sealed class SaleItemCommand
{
    public Guid ProductExternalId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
