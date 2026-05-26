using Ambev.DeveloperEvaluation.Application.Common.Metrics;

namespace Ambev.DeveloperEvaluation.WebApi.Observability;

public sealed class NoOpApplicationMetrics : IApplicationMetrics
{
    public void SaleCreated() { }
    public void SaleModified() { }
    public void SaleCancelled() { }
    public void SaleItemCancelled() { }
    public void BusinessRuleViolation() { }
    public void CacheHit(string cacheKeyType) { }
    public void CacheMiss(string cacheKeyType) { }
    public void CacheError(string cacheKeyType) { }
    public void CacheInvalidated(string cacheKeyType) { }
}
