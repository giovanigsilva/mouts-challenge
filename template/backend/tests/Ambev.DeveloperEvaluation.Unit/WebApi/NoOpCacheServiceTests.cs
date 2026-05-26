using Ambev.DeveloperEvaluation.WebApi.Caching;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

public class NoOpCacheServiceTests
{
    [Fact(DisplayName = "NoOp cache should call factory on GetOrCreate")]
    public async Task Given_NoOpCache_When_GetOrCreate_Then_ShouldCallFactory()
    {
        var cache = new NoOpCacheService();
        var factoryCalls = 0;

        var result = await cache.GetOrCreateAsync("key", "type", _ =>
        {
            factoryCalls++;
            return Task.FromResult("value");
        }, CancellationToken.None);

        result.Should().Be("value");
        factoryCalls.Should().Be(1);
    }

    [Fact(DisplayName = "NoOp cache should return default on Get")]
    public async Task Given_NoOpCache_When_Get_Then_ShouldReturnDefault()
    {
        var cache = new NoOpCacheService();

        var result = await cache.GetAsync<string>("key", "type", CancellationToken.None);

        result.Should().BeNull();
    }
}
