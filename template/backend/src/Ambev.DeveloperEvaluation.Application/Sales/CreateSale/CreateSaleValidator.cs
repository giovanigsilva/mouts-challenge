using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public sealed class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleValidator()
    {
        RuleFor(sale => sale.SaleNumber).NotEmpty().MaximumLength(50);
        RuleFor(sale => sale.SaleDate).NotEmpty();
        RuleFor(sale => sale.CustomerExternalId).NotEmpty();
        RuleFor(sale => sale.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(sale => sale.BranchExternalId).NotEmpty();
        RuleFor(sale => sale.BranchName).NotEmpty().MaximumLength(200);
        RuleFor(sale => sale.Items).NotEmpty();
        RuleForEach(sale => sale.Items).ChildRules(item =>
        {
            item.RuleFor(value => value.ProductExternalId).NotEmpty();
            item.RuleFor(value => value.ProductName).NotEmpty().MaximumLength(200);
            item.RuleFor(value => value.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
            item.RuleFor(value => value.UnitPrice).GreaterThan(0);
        });
    }
}
