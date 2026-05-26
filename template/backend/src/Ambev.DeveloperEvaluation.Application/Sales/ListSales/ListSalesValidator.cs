using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesValidator : AbstractValidator<ListSalesQuery>
{
    public ListSalesValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1).WithMessage("A pagina deve ser maior ou igual a 1.");
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100).WithMessage("O tamanho da pagina deve estar entre 1 e 100.");
        RuleFor(query => query)
            .Must(query => !query.FromDate.HasValue || !query.ToDate.HasValue || query.FromDate.Value <= query.ToDate.Value)
            .WithMessage("A data inicial deve ser menor ou igual a data final.");
    }
}
