using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SalesValidatorTests
{
    [Fact(DisplayName = "CreateSaleValidator should reject empty items")]
    public void Given_EmptyItems_When_ValidatingCreateSale_Then_ShouldHaveError()
    {
        var validator = new CreateSaleValidator();
        var command = CreateValidCommand();
        command.Items = [];

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.Items);
    }

    [Fact(DisplayName = "CreateSaleValidator should reject invalid quantity")]
    public void Given_InvalidQuantity_When_ValidatingCreateSale_Then_ShouldHaveError()
    {
        var validator = new CreateSaleValidator();
        var command = CreateValidCommand();
        command.Items = [CreateValidItem(quantity: 21)];

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact(DisplayName = "CreateSaleValidator should reject invalid unit price")]
    public void Given_InvalidUnitPrice_When_ValidatingCreateSale_Then_ShouldHaveError()
    {
        var validator = new CreateSaleValidator();
        var command = CreateValidCommand();
        command.Items = [CreateValidItem(unitPrice: 0m)];

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Items[0].UnitPrice");
    }

    [Fact(DisplayName = "CreateSaleValidator should reject duplicated product")]
    public void Given_DuplicatedProduct_When_ValidatingCreateSale_Then_ShouldHaveError()
    {
        var validator = new CreateSaleValidator();
        var productId = Guid.NewGuid();
        var command = CreateValidCommand();
        command.Items =
        [
            CreateValidItem(productId),
            CreateValidItem(productId)
        ];

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.Items);
    }

    [Fact(DisplayName = "ListSalesValidator should reject invalid pagination")]
    public void Given_InvalidPagination_When_ValidatingListSales_Then_ShouldHaveErrors()
    {
        var validator = new ListSalesValidator();
        var query = new ListSalesQuery { Page = 0, PageSize = 101 };

        var result = validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(query => query.Page);
        result.ShouldHaveValidationErrorFor(query => query.PageSize);
    }

    [Fact(DisplayName = "ListSalesValidator should reject invalid date range")]
    public void Given_InvalidDateRange_When_ValidatingListSales_Then_ShouldHaveError()
    {
        var validator = new ListSalesValidator();
        var query = new ListSalesQuery { FromDate = DateTime.UtcNow.Date.AddDays(1), ToDate = DateTime.UtcNow.Date };

        var result = validator.TestValidate(query);

        result.ShouldHaveAnyValidationError();
    }

    [Fact(DisplayName = "CancelSaleItemValidator should reject empty ids")]
    public void Given_EmptyIds_When_ValidatingCancelSaleItem_Then_ShouldHaveErrors()
    {
        var validator = new CancelSaleItemValidator();
        var command = new CancelSaleItemCommand();

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.SaleId);
        result.ShouldHaveValidationErrorFor(command => command.ItemId);
    }

    private static CreateSaleCommand CreateValidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = "SALE-1",
            SaleDate = DateTime.UtcNow,
            CustomerExternalId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchExternalId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = [CreateValidItem()]
        };
    }

    private static SaleItemInput CreateValidItem(Guid? productId = null, int quantity = 1, decimal unitPrice = 10m)
    {
        return new SaleItemInput
        {
            ProductExternalId = productId ?? Guid.NewGuid(),
            ProductName = "Produto",
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
