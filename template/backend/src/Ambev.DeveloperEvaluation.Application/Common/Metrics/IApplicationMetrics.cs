namespace Ambev.DeveloperEvaluation.Application.Common.Metrics;

public interface IApplicationMetrics
{
    void SaleCreated();
    void SaleModified();
    void SaleCancelled();
    void SaleItemCancelled();
    void BusinessRuleViolation();
    void CacheHit(string cacheKeyType);
    void CacheMiss(string cacheKeyType);
    void CacheError(string cacheKeyType);
    void CacheInvalidated(string cacheKeyType);
}
