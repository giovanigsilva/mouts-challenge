using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Common.Metrics;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISalesCacheInvalidator _salesCacheInvalidator;
    private readonly IApplicationMetrics _metrics;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(ISaleRepository saleRepository, ISalesCacheInvalidator salesCacheInvalidator, IApplicationMetrics metrics, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _salesCacheInvalidator = salesCacheInvalidator;
        _metrics = metrics;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} SaleNumber={SaleNumber} OccurredAtUtc={OccurredAtUtc}", "SaleUpdateStarted", "UpdateSale", "Started", "Sale", command.Id, command.SaleNumber, DateTime.UtcNow);

        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
        {
            _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "SaleUpdateFailed", "UpdateSale", "NotFound", "Sale", command.Id, DateTime.UtcNow);
            throw new KeyNotFoundException($"Venda com ID {command.Id} nao encontrada.");
        }

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale is not null && existingSale.Id != command.Id)
        {
            _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} SaleNumber={SaleNumber} OccurredAtUtc={OccurredAtUtc}", "SaleUpdateFailed", "UpdateSale", "DuplicatedSaleNumber", "Sale", command.Id, command.SaleNumber, DateTime.UtcNow);
            throw new InvalidOperationException($"Venda com numero {command.SaleNumber} ja existe.");
        }

        var items = command.Items.Select(item => new SaleItem(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice));
        sale.Update(command.SaleNumber, command.SaleDate, command.CustomerExternalId, command.CustomerName, command.BranchExternalId, command.BranchName, command.UpdatedByUserId, items);
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _salesCacheInvalidator.InvalidateAsync(updatedSale.Id, "UpdateSale", cancellationToken);
        _metrics.SaleModified();
        try
        {
            _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} SaleId={SaleId} SaleNumber={SaleNumber} CustomerExternalId={CustomerExternalId} BranchExternalId={BranchExternalId} TotalAmount={TotalAmount} OccurredAtUtc={OccurredAtUtc}", "SaleUpdated", "UpdateSale", "Success", "Sale", updatedSale.Id, updatedSale.Id, updatedSale.SaleNumber, updatedSale.CustomerExternalId, updatedSale.BranchExternalId, updatedSale.TotalAmount, DateTime.UtcNow);
        }
        catch (Exception logException)
        {
            _logger.LogWarning(logException, "Falha ao registrar evento SaleModified em log. SaleId={SaleId}", updatedSale.Id);
        }

        return _mapper.Map<SaleResult>(updatedSale);
    }
}
