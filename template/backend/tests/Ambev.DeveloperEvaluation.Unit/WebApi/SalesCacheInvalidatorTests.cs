using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.WebApi.Caching;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

public class SalesCacheInvalidatorTests
{
    [Fact(DisplayName = "Sales cache invalidator should remove detail and increment list version")]
    public async Task Given_SaleId_When_Invalidating_Then_ShouldRemoveDetailsAndIncrementListVersion()
    {
        var saleId = Guid.NewGuid();
        var cache = Substitute.For<ICacheService>();
        var invalidator = new SalesCacheInvalidator(cache, NullLogger<SalesCacheInvalidator>.Instance);

        await invalidator.InvalidateAsync(saleId, "UpdateSale", CancellationToken.None);

        await cache.Received(1).RemoveAsync(SalesCacheKeys.Details(saleId), SalesCacheKeys.DetailsKeyType, CancellationToken.None);
        await cache.Received(1).IncrementAsync(SalesCacheKeys.ListVersionKey, SalesCacheKeys.ListVersionKeyType, CancellationToken.None);
    }
}
