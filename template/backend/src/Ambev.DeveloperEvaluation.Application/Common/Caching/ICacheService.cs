namespace Ambev.DeveloperEvaluation.Application.Common.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, string cacheKeyType, CancellationToken cancellationToken);

    Task SetAsync<T>(string key, T value, string cacheKeyType, CancellationToken cancellationToken);

    Task<T> GetOrCreateAsync<T>(string key, string cacheKeyType, Func<CancellationToken, Task<T>> factory, CancellationToken cancellationToken);

    Task RemoveAsync(string key, string cacheKeyType, CancellationToken cancellationToken);

    Task<long> IncrementAsync(string key, string cacheKeyType, CancellationToken cancellationToken);
}
