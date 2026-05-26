using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleValidator : AbstractValidator<GetSaleQuery>
{
    public GetSaleValidator()
    {
        RuleFor(query => query.Id).NotEmpty().WithMessage("O identificador da venda e obrigatorio.");
    }
}
