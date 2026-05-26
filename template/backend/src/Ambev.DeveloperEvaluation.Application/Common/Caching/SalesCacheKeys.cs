using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.Application.Common.Caching;

public static class SalesCacheKeys
{
    public const string DetailsKeyType = "sales-details";
    public const string ListKeyType = "sales-list";
    public const string ListVersionKeyType = "sales-list-version";
    public const string ListVersionKey = "sales:v1:list:version";

    public static string Details(Guid saleId) => $"sales:v1:details:{saleId}";

    public static string List(ListSalesQuery query, long version)
    {
        return string.Join(':',
            "sales",
            "v1",
            "list",
            $"v{version}",
            $"page:{query.Page}",
            $"size:{query.PageSize}",
            $"saleNumber:{query.SaleNumber ?? "all"}",
            $"customer:{query.CustomerId?.ToString() ?? "all"}",
            $"branch:{query.BranchId?.ToString() ?? "all"}",
            $"cancelled:{query.IsCancelled?.ToString() ?? "all"}",
            $"from:{query.FromDate?.ToString("O") ?? "all"}",
            $"to:{query.ToDate?.ToString("O") ?? "all"}");
    }
}
