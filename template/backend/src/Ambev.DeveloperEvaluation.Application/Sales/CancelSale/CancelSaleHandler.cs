using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISalesCacheInvalidator _salesCacheInvalidator;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(ISaleRepository saleRepository, ISalesCacheInvalidator salesCacheInvalidator, IMapper mapper, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _salesCacheInvalidator = salesCacheInvalidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
        {
            _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "SaleCancelFailed", "CancelSale", "NotFound", "Sale", command.Id, DateTime.UtcNow);
            throw new KeyNotFoundException($"Venda com ID {command.Id} nao encontrada.");
        }

        sale.Cancel();
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _salesCacheInvalidator.InvalidateAsync(updatedSale.Id, "CancelSale", cancellationToken);
        try
        {
            _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} SaleId={SaleId} SaleNumber={SaleNumber} CustomerExternalId={CustomerExternalId} BranchExternalId={BranchExternalId} TotalAmount={TotalAmount} OccurredAtUtc={OccurredAtUtc}", "SaleCancelled", "CancelSale", "Success", "Sale", updatedSale.Id, updatedSale.Id, updatedSale.SaleNumber, updatedSale.CustomerExternalId, updatedSale.BranchExternalId, updatedSale.TotalAmount, DateTime.UtcNow);
        }
        catch (Exception logException)
        {
            _logger.LogWarning(logException, "Falha ao registrar evento SaleCancelled em log. SaleId={SaleId}", updatedSale.Id);
        }

        return _mapper.Map<SaleResult>(updatedSale);
    }
}
