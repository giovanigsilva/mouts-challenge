using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public sealed class SaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public SaleRequestValidator()
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
