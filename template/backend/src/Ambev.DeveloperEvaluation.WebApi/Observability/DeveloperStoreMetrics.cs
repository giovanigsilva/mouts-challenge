using System.Diagnostics.Metrics;
using Ambev.DeveloperEvaluation.Application.Common.Metrics;

namespace Ambev.DeveloperEvaluation.WebApi.Observability;

public sealed class DeveloperStoreMetrics : IApplicationMetrics
{
    public const string MeterName = "DeveloperStore.Sales.Api";

    private readonly Counter<long> _salesCreated;
    private readonly Counter<long> _salesModified;
    private readonly Counter<long> _salesCancelled;
    private readonly Counter<long> _salesItemCancelled;
    private readonly Counter<long> _salesBusinessRuleViolation;
    private readonly Counter<long> _cacheHit;
    private readonly Counter<long> _cacheMiss;
    private readonly Counter<long> _cacheError;
    private readonly Counter<long> _cacheInvalidation;

    public DeveloperStoreMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _salesCreated = meter.CreateCounter<long>("developerstore_sales_created_total");
        _salesModified = meter.CreateCounter<long>("developerstore_sales_modified_total");
        _salesCancelled = meter.CreateCounter<long>("developerstore_sales_cancelled_total");
        _salesItemCancelled = meter.CreateCounter<long>("developerstore_sales_item_cancelled_total");
        _salesBusinessRuleViolation = meter.CreateCounter<long>("developerstore_sales_business_rule_violation_total");
        _cacheHit = meter.CreateCounter<long>("developerstore_cache_hit_total");
        _cacheMiss = meter.CreateCounter<long>("developerstore_cache_miss_total");
        _cacheError = meter.CreateCounter<long>("developerstore_cache_error_total");
        _cacheInvalidation = meter.CreateCounter<long>("developerstore_cache_invalidation_total");
    }

    public void SaleCreated() => _salesCreated.Add(1);

    public void SaleModified() => _salesModified.Add(1);

    public void SaleCancelled() => _salesCancelled.Add(1);

    public void SaleItemCancelled() => _salesItemCancelled.Add(1);

    public void BusinessRuleViolation() => _salesBusinessRuleViolation.Add(1);

    public void CacheHit(string cacheKeyType) => _cacheHit.Add(1, new KeyValuePair<string, object?>("cache_key_type", cacheKeyType));

    public void CacheMiss(string cacheKeyType) => _cacheMiss.Add(1, new KeyValuePair<string, object?>("cache_key_type", cacheKeyType));

    public void CacheError(string cacheKeyType) => _cacheError.Add(1, new KeyValuePair<string, object?>("cache_key_type", cacheKeyType));

    public void CacheInvalidated(string cacheKeyType) => _cacheInvalidation.Add(1, new KeyValuePair<string, object?>("cache_key_type", cacheKeyType));
}
