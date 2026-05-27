using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Theory(DisplayName = "Quantity discount tiers should be applied by domain")]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 10)]
    [InlineData(9, 10)]
    [InlineData(10, 20)]
    [InlineData(20, 20)]
    public void Given_Quantity_When_CreatingSaleItem_Then_DiscountShouldBeApplied(int quantity, decimal expectedDiscount)
    {
        var item = CreateItem(quantity, 100m);

        item.DiscountPercentage.Should().Be(expectedDiscount);
    }

    [Fact(DisplayName = "Quantity above twenty should throw business rule exception")]
    public void Given_QuantityAboveTwenty_When_CreatingSaleItem_Then_ShouldThrowBusinessRuleException()
    {
        var action = () => CreateItem(21, 100m);

        action.Should().Throw<BusinessRuleException>();
    }

    [Fact(DisplayName = "Duplicated product in sale should be rejected")]
    public void Given_DuplicatedProduct_When_CreatingSale_Then_ShouldThrowBusinessRuleException()
    {
        var productId = Guid.NewGuid();
        var items = new[]
        {
            new SaleItem(productId, "Produto A", 1, 10m),
            new SaleItem(productId, "Produto A", 2, 10m)
        };

        var action = () => CreateSale(items);

        action.Should().Throw<BusinessRuleException>();
    }

    [Fact(DisplayName = "Cancelled item should not compose sale total")]
    public void Given_CancelledItem_When_TotalIsCalculated_Then_ItemShouldNotComposeTotal()
    {
        var firstItem = CreateItem(4, 100m);
        var secondItem = CreateItem(1, 50m);
        var sale = CreateSale([firstItem, secondItem]);

        sale.CancelItem(firstItem.Id, Guid.NewGuid());

        sale.TotalAmount.Should().Be(50m);
    }

    [Fact(DisplayName = "Cancelled sale cannot be modified")]
    public void Given_CancelledSale_When_UpdatingSale_Then_ShouldThrowBusinessRuleException()
    {
        var sale = CreateSale([CreateItem(1, 100m)]);
        sale.Cancel(Guid.NewGuid());

        var action = () => sale.Update("SALE-2", DateTime.UtcNow, Guid.NewGuid(), "Cliente 2", Guid.NewGuid(), "Filial 2", Guid.NewGuid(), [CreateItem(1, 100m)]);

        action.Should().Throw<BusinessRuleException>();
    }

    [Fact(DisplayName = "Sale total should be recalculated after update")]
    public void Given_UpdatedItems_When_UpdatingSale_Then_TotalShouldBeRecalculated()
    {
        var sale = CreateSale([CreateItem(1, 100m)]);

        sale.Update("SALE-1", DateTime.UtcNow, Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial", Guid.NewGuid(), [CreateItem(10, 100m)]);

        sale.TotalAmount.Should().Be(800m);
    }

    [Fact(DisplayName = "SaleCreatedEvent should be registered on creation")]
    public void Given_NewSale_When_Created_Then_ShouldRegisterSaleCreatedEvent()
    {
        var sale = CreateSale([CreateItem(1, 100m)]);

        sale.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is SaleCreatedEvent);
    }

    [Fact(DisplayName = "SaleModifiedEvent should be registered on update")]
    public void Given_Sale_When_Updated_Then_ShouldRegisterSaleModifiedEvent()
    {
        var sale = CreateSale([CreateItem(1, 100m)]);

        sale.Update("SALE-2", DateTime.UtcNow, Guid.NewGuid(), "Cliente 2", Guid.NewGuid(), "Filial 2", Guid.NewGuid(), [CreateItem(2, 100m)]);

        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleModifiedEvent);
    }

    [Fact(DisplayName = "SaleCancelledEvent should be registered on cancellation")]
    public void Given_Sale_When_Cancelled_Then_ShouldRegisterSaleCancelledEvent()
    {
        var sale = CreateSale([CreateItem(1, 100m)]);

        sale.Cancel(Guid.NewGuid());

        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleCancelledEvent);
    }

    [Fact(DisplayName = "ItemCancelledEvent should be registered on item cancellation")]
    public void Given_SaleItem_When_Cancelled_Then_ShouldRegisterItemCancelledEvent()
    {
        var item = CreateItem(1, 100m);
        var sale = CreateSale([item]);

        sale.CancelItem(item.Id, Guid.NewGuid());

        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is ItemCancelledEvent);
    }

    private static SaleItem CreateItem(int quantity, decimal unitPrice)
    {
        return new SaleItem(Guid.NewGuid(), "Produto", quantity, unitPrice);
    }

    private static Sale CreateSale(IEnumerable<SaleItem> items)
    {
        return new Sale("SALE-1", DateTime.UtcNow, Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial", Guid.NewGuid(), items);
    }
}
