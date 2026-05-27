using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemValidator : AbstractValidator<CancelSaleItemCommand>
{
    public CancelSaleItemValidator()
    {
        RuleFor(command => command.SaleId).NotEmpty().WithMessage("O identificador da venda e obrigatorio.");
        RuleFor(command => command.ItemId).NotEmpty().WithMessage("O identificador do item e obrigatorio.");
        RuleFor(command => command.UpdatedByUserId).NotEmpty().WithMessage("O usuario responsavel pelo cancelamento do item e obrigatorio.");
    }
}
