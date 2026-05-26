using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public sealed class ListSalesHandler : IRequestHandler<ListSalesCommand, PaginatedSalesResult>
{
    private const int MaxPageSize = 100;
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCacheService _saleCacheService;

    public ListSalesHandler(ISaleRepository saleRepository, ISaleCacheService saleCacheService)
    {
        _saleRepository = saleRepository;
        _saleCacheService = saleCacheService;
    }

    public async Task<PaginatedSalesResult> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, MaxPageSize);
        request.Page = page;
        request.PageSize = pageSize;

        var cachedResult = await _saleCacheService.GetSalesAsync(request, cancellationToken);
        if (cachedResult != null)
            return cachedResult;

        var count = await _saleRepository.CountAsync(request.SaleNumber, request.CustomerId, request.BranchId, request.IsCancelled, request.FromDate, request.ToDate, cancellationToken);
        var sales = await _saleRepository.ListAsync(request.SaleNumber, request.CustomerId, request.BranchId, request.IsCancelled, request.FromDate, request.ToDate, page, pageSize, cancellationToken);

        var result = new PaginatedSalesResult
        {
            Items = sales.Select(SaleResultMapper.Map).ToList(),
            CurrentPage = page,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };

        await _saleCacheService.SetSalesAsync(request, result, cancellationToken);
        return result;
    }
}
