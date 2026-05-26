using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesQuery, PaginatedSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<ListSalesHandler> _logger;

    public ListSalesHandler(ISaleRepository saleRepository, ICacheService cacheService, IMapper mapper, ILogger<ListSalesHandler> logger)
    {
        _saleRepository = saleRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedSalesResult> Handle(ListSalesQuery query, CancellationToken cancellationToken)
    {
        var validator = new ListSalesValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var version = await _cacheService.GetAsync<long?>(SalesCacheKeys.ListVersionKey, SalesCacheKeys.ListVersionKeyType, cancellationToken) ?? 1L;

        return await _cacheService.GetOrCreateAsync(
            SalesCacheKeys.List(query, version),
            SalesCacheKeys.ListKeyType,
            async token =>
            {
                var result = await _saleRepository.ListAsync(query.Page, query.PageSize, query.SaleNumber, query.CustomerId, query.BranchId, query.IsCancelled, query.FromDate, query.ToDate, token);
                _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} Page={Page} PageSize={PageSize} TotalCount={TotalCount} SaleNumber={SaleNumber} CustomerId={CustomerId} BranchId={BranchId} OccurredAtUtc={OccurredAtUtc}", "SalesListed", "ListSales", "Success", "Sale", query.Page, query.PageSize, result.TotalCount, query.SaleNumber, query.CustomerId, query.BranchId, DateTime.UtcNow);

                return new PaginatedSalesResult
                {
                    Items = _mapper.Map<IEnumerable<SaleResult>>(result.Items),
                    CurrentPage = query.Page,
                    TotalCount = result.TotalCount,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)query.PageSize)
                };
            },
            cancellationToken);
    }
}
