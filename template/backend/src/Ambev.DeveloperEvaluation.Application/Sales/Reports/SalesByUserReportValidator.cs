using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Reports;

public class SalesByUserReportValidator : AbstractValidator<SalesByUserReportQuery>
{
    public SalesByUserReportValidator()
    {
        RuleFor(query => query)
            .Must(query => !query.FromDate.HasValue || !query.ToDate.HasValue || query.FromDate <= query.ToDate)
            .WithMessage("A data inicial deve ser menor ou igual a data final.");
    }
}
