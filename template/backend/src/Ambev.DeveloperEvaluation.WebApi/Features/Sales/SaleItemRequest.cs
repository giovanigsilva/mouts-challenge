namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public sealed class SaleItemRequest
{
    public Guid ProductExternalId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
