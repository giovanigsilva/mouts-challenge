namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public sealed class PaginatedSalesResult
{
    public IReadOnlyCollection<SaleResult> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
