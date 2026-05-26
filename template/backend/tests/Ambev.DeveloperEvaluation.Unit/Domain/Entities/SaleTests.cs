using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public sealed class SaleTests
{
    [Theory(DisplayName = "Given quantity from one to three When adding item Then applies zero discount")]
    [InlineData(1)]
    [InlineData(3)]
    public void AddItem_QuantityFromOneToThree_AppliesZeroDiscount(int quantity)
    {
        var sale = CreateSale();

        var item = sale.AddItem(Guid.NewGuid(), "Produto", quantity, 10m);

        item.DiscountPercentage.Should().Be(0m);
        item.DiscountAmount.Should().Be(0m);
        item.TotalAmount.Should().Be(quantity * 10m);
    }

    [Theory(DisplayName = "Given quantity from four to nine When adding item Then applies ten percent discount")]
    [InlineData(4)]
    [InlineData(9)]
    public void AddItem_QuantityFromFourToNine_AppliesTenPercentDiscount(int quantity)
    {
        var sale = CreateSale();

        var item = sale.AddItem(Guid.NewGuid(), "Produto", quantity, 10m);

        item.DiscountPercentage.Should().Be(10m);
        item.DiscountAmount.Should().Be(quantity);
        item.TotalAmount.Should().Be(quantity * 10m - quantity);
    }

    [Theory(DisplayName = "Given quantity from ten to twenty When adding item Then applies twenty percent discount")]
    [InlineData(10)]
    [InlineData(20)]
    public void AddItem_QuantityFromTenToTwenty_AppliesTwentyPercentDiscount(int quantity)
    {
        var sale = CreateSale();

        var item = sale.AddItem(Guid.NewGuid(), "Produto", quantity, 10m);

        item.DiscountPercentage.Should().Be(20m);
        item.DiscountAmount.Should().Be(quantity * 2m);
        item.TotalAmount.Should().Be(quantity * 8m);
    }

    [Fact(DisplayName = "Given quantity above twenty When adding item Then throws business rule exception")]
    public void AddItem_QuantityAboveTwenty_ThrowsBusinessRuleException()
    {
        var sale = CreateSale();

        var act = () => sale.AddItem(Guid.NewGuid(), "Produto", 21, 10m);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact(DisplayName = "Given cancelled item When calculating sale total Then item does not compose total")]
    public void CancelItem_WhenItemIsCancelled_RemovesItemFromTotal()
    {
        var sale = CreateSale();
        var firstItem = sale.AddItem(Guid.NewGuid(), "Produto 1", 4, 10m);
        sale.AddItem(Guid.NewGuid(), "Produto 2", 1, 10m);

        sale.CancelItem(firstItem.Id);

        sale.TotalAmount.Should().Be(10m);
    }

    [Fact(DisplayName = "Given cancelled sale When adding item Then throws business rule exception")]
    public void AddItem_WhenSaleIsCancelled_ThrowsBusinessRuleException()
    {
        var sale = CreateSale();
        sale.Cancel();

        var act = () => sale.AddItem(Guid.NewGuid(), "Produto", 1, 10m);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact(DisplayName = "Given sale item changes When replacing items Then recalculates total")]
    public void ReplaceItems_WhenItemsChange_RecalculatesTotal()
    {
        var sale = CreateSale();
        sale.AddItem(Guid.NewGuid(), "Produto", 1, 10m);

        sale.ReplaceItems([new SaleItemInput(Guid.NewGuid(), "Produto atualizado", 10, 10m)]);

        sale.TotalAmount.Should().Be(80m);
    }

    [Fact(DisplayName = "Given new sale When created Then raises SaleCreatedEvent")]
    public void Constructor_WhenSaleIsCreated_RaisesSaleCreatedEvent()
    {
        var sale = CreateSale();

        sale.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is SaleCreatedEvent);
    }

    [Fact(DisplayName = "Given existing sale When updated Then raises SaleModifiedEvent")]
    public void Update_WhenSaleIsUpdated_RaisesSaleModifiedEvent()
    {
        var sale = CreateSale();

        sale.Update("S-002", DateTime.UtcNow, Guid.NewGuid(), "Cliente 2", Guid.NewGuid(), "Filial 2");

        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleModifiedEvent);
    }

    [Fact(DisplayName = "Given existing sale When cancelled Then raises SaleCancelledEvent")]
    public void Cancel_WhenSaleIsCancelled_RaisesSaleCancelledEvent()
    {
        var sale = CreateSale();

        sale.Cancel();

        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleCancelledEvent);
    }

    [Fact(DisplayName = "Given sale item When item is cancelled Then raises ItemCancelledEvent")]
    public void CancelItem_WhenItemIsCancelled_RaisesItemCancelledEvent()
    {
        var sale = CreateSale();
        var item = sale.AddItem(Guid.NewGuid(), "Produto", 1, 10m);

        sale.CancelItem(item.Id);

        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is ItemCancelledEvent);
    }

    private static Sale CreateSale()
    {
        return new Sale("S-001", DateTime.UtcNow, Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial");
    }
}
