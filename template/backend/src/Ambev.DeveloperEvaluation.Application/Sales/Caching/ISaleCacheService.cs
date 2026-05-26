using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Caching;

public interface ISaleCacheService
{
    Task<SaleResult?> GetSaleAsync(Guid saleId, CancellationToken cancellationToken);
    Task SetSaleAsync(SaleResult sale, CancellationToken cancellationToken);
    Task<PaginatedSalesResult?> GetSalesAsync(ListSalesCommand query, CancellationToken cancellationToken);
    Task SetSalesAsync(ListSalesCommand query, PaginatedSalesResult result, CancellationToken cancellationToken);
    Task InvalidateSaleAsync(Guid saleId, CancellationToken cancellationToken);
    Task InvalidateSalesListAsync(CancellationToken cancellationToken);
}
