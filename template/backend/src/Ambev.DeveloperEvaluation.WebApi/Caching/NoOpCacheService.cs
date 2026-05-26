using Ambev.DeveloperEvaluation.Application.Common.Caching;

namespace Ambev.DeveloperEvaluation.WebApi.Caching;

public sealed class NoOpCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, string cacheKeyType, CancellationToken cancellationToken)
    {
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, string cacheKeyType, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<T> GetOrCreateAsync<T>(string key, string cacheKeyType, Func<CancellationToken, Task<T>> factory, CancellationToken cancellationToken)
    {
        return factory(cancellationToken);
    }

    public Task RemoveAsync(string key, string cacheKeyType, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<long> IncrementAsync(string key, string cacheKeyType, CancellationToken cancellationToken)
    {
        return Task.FromResult(1L);
    }
}
