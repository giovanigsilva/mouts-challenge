using Ambev.DeveloperEvaluation.Application.Sales.Common;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleValidator()
    {
        RuleFor(sale => sale.Id).NotEmpty().WithMessage("O identificador da venda e obrigatorio.");
        RuleFor(sale => sale.SaleNumber).NotEmpty().MaximumLength(60).WithMessage("O numero da venda e obrigatorio e deve ter no maximo 60 caracteres.");
        RuleFor(sale => sale.SaleDate).NotEmpty().WithMessage("A data da venda e obrigatoria.");
        RuleFor(sale => sale.CustomerExternalId).NotEmpty().WithMessage("O identificador externo do cliente e obrigatorio.");
        RuleFor(sale => sale.CustomerName).NotEmpty().MaximumLength(120).WithMessage("O nome do cliente e obrigatorio e deve ter no maximo 120 caracteres.");
        RuleFor(sale => sale.BranchExternalId).NotEmpty().WithMessage("O identificador externo da filial e obrigatorio.");
        RuleFor(sale => sale.BranchName).NotEmpty().MaximumLength(120).WithMessage("O nome da filial e obrigatorio e deve ter no maximo 120 caracteres.");
        RuleFor(sale => sale.UpdatedByUserId).NotEmpty().WithMessage("O usuario responsavel pela atualizacao da venda e obrigatorio.");
        RuleFor(sale => sale.Items).NotEmpty().WithMessage("A venda deve possuir ao menos um item.");
        RuleForEach(sale => sale.Items).SetValidator(new SaleItemInputValidator());
        RuleFor(sale => sale.Items)
            .Must(items => items is not null && items.Select(item => item.ProductExternalId).Distinct().Count() == items.Count())
            .WithMessage("Nao e permitido repetir o mesmo produto na venda.");
    }
}
