namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public sealed class UpdateSaleRequest
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerExternalId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchExternalId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public IReadOnlyCollection<SaleItemRequest> Items { get; set; } = [];
}
