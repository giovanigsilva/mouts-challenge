using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

public class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public SaleItemRequestValidator()
    {
        RuleFor(item => item.ProductExternalId).NotEmpty().WithMessage("O identificador externo do produto e obrigatorio.");
        RuleFor(item => item.ProductName).NotEmpty().MaximumLength(120).WithMessage("O nome do produto e obrigatorio e deve ter no maximo 120 caracteres.");
        RuleFor(item => item.Quantity).InclusiveBetween(1, 20).WithMessage("A quantidade deve estar entre 1 e 20.");
        RuleFor(item => item.UnitPrice).GreaterThan(0).WithMessage("O preco unitario deve ser maior que zero.");
    }
}
