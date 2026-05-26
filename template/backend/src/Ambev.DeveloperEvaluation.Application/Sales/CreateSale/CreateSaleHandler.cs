using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Common.Metrics;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISalesCacheInvalidator _salesCacheInvalidator;
    private readonly IApplicationMetrics _metrics;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository saleRepository, ISalesCacheInvalidator salesCacheInvalidator, IApplicationMetrics metrics, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _salesCacheInvalidator = salesCacheInvalidator;
        _metrics = metrics;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} SaleNumber={SaleNumber} CustomerExternalId={CustomerExternalId} BranchExternalId={BranchExternalId} OccurredAtUtc={OccurredAtUtc}", "SaleCreateStarted", "CreateSale", "Started", "Sale", command.SaleNumber, command.CustomerExternalId, command.BranchExternalId, DateTime.UtcNow);

        var validator = new CreateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale is not null)
        {
            _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} SaleNumber={SaleNumber} OccurredAtUtc={OccurredAtUtc}", "SaleCreateFailed", "CreateSale", "DuplicatedSaleNumber", "Sale", command.SaleNumber, DateTime.UtcNow);
            throw new InvalidOperationException($"Venda com numero {command.SaleNumber} ja existe.");
        }

        var items = command.Items.Select(item => new SaleItem(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice));
        var sale = new Sale(command.SaleNumber, command.SaleDate, command.CustomerExternalId, command.CustomerName, command.BranchExternalId, command.BranchName, items);
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        await _salesCacheInvalidator.InvalidateAsync(createdSale.Id, "CreateSale", cancellationToken);
        _metrics.SaleCreated();
        try
        {
            _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} SaleId={SaleId} SaleNumber={SaleNumber} CustomerExternalId={CustomerExternalId} BranchExternalId={BranchExternalId} TotalAmount={TotalAmount} OccurredAtUtc={OccurredAtUtc}", "SaleCreated", "CreateSale", "Success", "Sale", createdSale.Id, createdSale.Id, createdSale.SaleNumber, createdSale.CustomerExternalId, createdSale.BranchExternalId, createdSale.TotalAmount, DateTime.UtcNow);
        }
        catch (Exception logException)
        {
            _logger.LogWarning(logException, "Falha ao registrar evento SaleCreated em log. SaleId={SaleId}", createdSale.Id);
        }

        return _mapper.Map<SaleResult>(createdSale);
    }
}
