using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Reports;

public class SalesByUserReportHandler : IRequestHandler<SalesByUserReportQuery, IEnumerable<SalesByUserReportResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SalesByUserReportHandler> _logger;

    public SalesByUserReportHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<SalesByUserReportHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<SalesByUserReportResult>> Handle(SalesByUserReportQuery query, CancellationToken cancellationToken)
    {
        var validator = new SalesByUserReportValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var report = await _saleRepository.GetSalesByUserReportAsync(query.FromDate, query.ToDate, cancellationToken);
        _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} FromDate={FromDate} ToDate={ToDate} OccurredAtUtc={OccurredAtUtc}", "SalesByUserReportGenerated", "SalesByUserReport", "Success", "Sale", query.FromDate, query.ToDate, DateTime.UtcNow);

        return _mapper.Map<IEnumerable<SalesByUserReportResult>>(report);
    }
}
