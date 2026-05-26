using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.ORM.Cache;

public sealed class NoOpSaleCacheService : ISaleCacheService
{
    public Task<SaleResult?> GetSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        return Task.FromResult<SaleResult?>(null);
    }

    public Task SetSaleAsync(SaleResult sale, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<PaginatedSalesResult?> GetSalesAsync(ListSalesCommand query, CancellationToken cancellationToken)
    {
        return Task.FromResult<PaginatedSalesResult?>(null);
    }

    public Task SetSalesAsync(ListSalesCommand query, PaginatedSalesResult result, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task InvalidateSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task InvalidateSalesListAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
