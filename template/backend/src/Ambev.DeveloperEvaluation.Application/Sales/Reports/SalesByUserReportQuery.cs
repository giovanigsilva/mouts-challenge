using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Reports;

public class SalesByUserReportQuery : IRequest<IEnumerable<SalesByUserReportResult>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
