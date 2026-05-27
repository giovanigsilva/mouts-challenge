using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleValidator : AbstractValidator<CancelSaleCommand>
{
    public CancelSaleValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("O identificador da venda e obrigatorio.");
        RuleFor(command => command.CancelledByUserId).NotEmpty().WithMessage("O usuario responsavel pelo cancelamento da venda e obrigatorio.");
    }
}
