using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SalesCacheKeysTests
{
    [Fact(DisplayName = "Sales detail cache key should contain sale id")]
    public void Given_SaleId_When_BuildingDetailsKey_Then_ShouldContainSaleId()
    {
        var saleId = Guid.NewGuid();

        var key = SalesCacheKeys.Details(saleId);

        key.Should().Be($"sales:v1:details:{saleId}");
    }

    [Fact(DisplayName = "Sales list cache key should contain version and filters")]
    public void Given_Query_When_BuildingListKey_Then_ShouldContainVersionAndFilters()
    {
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var query = new ListSalesQuery
        {
            Page = 2,
            PageSize = 50,
            SaleNumber = "SALE-1",
            CustomerId = customerId,
            BranchId = branchId,
            IsCancelled = false,
            FromDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ToDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc)
        };

        var key = SalesCacheKeys.List(query, 7);

        key.Should().Contain("sales:v1:list:v7");
        key.Should().Contain("page:2");
        key.Should().Contain("size:50");
        key.Should().Contain("saleNumber:SALE-1");
        key.Should().Contain($"customer:{customerId}");
        key.Should().Contain($"branch:{branchId}");
        key.Should().Contain("cancelled:False");
    }
}
