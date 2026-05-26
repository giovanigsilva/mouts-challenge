using System.Text.Json;
using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Common.Metrics;
using Ambev.DeveloperEvaluation.IoC.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ambev.DeveloperEvaluation.IoC.Caching;

public sealed class DistributedCacheService : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IDistributedCache _cache;
    private readonly IApplicationMetrics _metrics;
    private readonly CacheOptions _options;
    private readonly ILogger<DistributedCacheService> _logger;

    public DistributedCacheService(IDistributedCache cache, IApplicationMetrics metrics, IOptions<CacheOptions> options, ILogger<DistributedCacheService> logger)
    {
        _cache = cache;
        _metrics = metrics;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, string cacheKeyType, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
            return default;

        try
        {
            var value = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrWhiteSpace(value))
            {
                _metrics.CacheMiss(cacheKeyType);
                _logger.LogInformation("CacheMiss CacheKeyType={CacheKeyType}", cacheKeyType);
                return default;
            }

            _metrics.CacheHit(cacheKeyType);
            _logger.LogInformation("CacheHit CacheKeyType={CacheKeyType}", cacheKeyType);
            return JsonSerializer.Deserialize<T>(value, SerializerOptions);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _metrics.CacheError(cacheKeyType);
            _logger.LogWarning(exception, "CacheError OperationName={OperationName} CacheKeyType={CacheKeyType} ExceptionType={ExceptionType}", "Get", cacheKeyType, exception.GetType().Name);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, string cacheKeyType, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
            return;

        try
        {
            var serializedValue = JsonSerializer.Serialize(value, SerializerOptions);
            await _cache.SetStringAsync(key, serializedValue, BuildEntryOptions(cacheKeyType), cancellationToken);
            _logger.LogInformation("CacheSet CacheKeyType={CacheKeyType}", cacheKeyType);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _metrics.CacheError(cacheKeyType);
            _logger.LogWarning(exception, "CacheError OperationName={OperationName} CacheKeyType={CacheKeyType} ExceptionType={ExceptionType}", "Set", cacheKeyType, exception.GetType().Name);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(string key, string cacheKeyType, Func<CancellationToken, Task<T>> factory, CancellationToken cancellationToken)
    {
        var cachedValue = await GetAsync<T>(key, cacheKeyType, cancellationToken);
        if (cachedValue is not null)
            return cachedValue;

        var value = await factory(cancellationToken);
        await SetAsync(key, value, cacheKeyType, cancellationToken);

        return value;
    }

    public async Task RemoveAsync(string key, string cacheKeyType, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
            return;

        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _metrics.CacheInvalidated(cacheKeyType);
            _logger.LogInformation("CacheInvalidated CacheKeyType={CacheKeyType}", cacheKeyType);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _metrics.CacheError(cacheKeyType);
            _logger.LogWarning(exception, "CacheError OperationName={OperationName} CacheKeyType={CacheKeyType} ExceptionType={ExceptionType}", "Remove", cacheKeyType, exception.GetType().Name);
        }
    }

    public async Task<long> IncrementAsync(string key, string cacheKeyType, CancellationToken cancellationToken)
    {
        var currentValue = await GetAsync<long?>(key, cacheKeyType, cancellationToken) ?? 0L;
        var nextValue = currentValue + 1;
        await SetAsync(key, nextValue, cacheKeyType, cancellationToken);

        return nextValue;
    }

    private DistributedCacheEntryOptions BuildEntryOptions(string cacheKeyType)
    {
        var expirationSeconds = cacheKeyType switch
        {
            SalesCacheKeys.DetailsKeyType => _options.SalesDetailExpirationSeconds,
            SalesCacheKeys.ListKeyType => _options.SalesListExpirationSeconds,
            _ => _options.DefaultExpirationSeconds
        };

        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirationSeconds)
        };
    }
}
