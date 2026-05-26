using Ambev.DeveloperEvaluation.Application.Common.Caching;

namespace Ambev.DeveloperEvaluation.WebApi.Caching;

public sealed class SalesCacheInvalidator : ISalesCacheInvalidator
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<SalesCacheInvalidator> _logger;

    public SalesCacheInvalidator(ICacheService cacheService, ILogger<SalesCacheInvalidator> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task InvalidateAsync(Guid saleId, string operationName, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync(SalesCacheKeys.Details(saleId), SalesCacheKeys.DetailsKeyType, cancellationToken);
        var version = await _cacheService.IncrementAsync(SalesCacheKeys.ListVersionKey, SalesCacheKeys.ListVersionKeyType, cancellationToken);

        _logger.LogInformation("CacheInvalidated OperationName={OperationName} SaleId={SaleId} ListVersion={ListVersion}", operationName, saleId, version);
    }
}
