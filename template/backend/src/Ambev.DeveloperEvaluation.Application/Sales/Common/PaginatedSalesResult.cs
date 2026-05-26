namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public class PaginatedSalesResult
{
    public IEnumerable<SaleResult> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
