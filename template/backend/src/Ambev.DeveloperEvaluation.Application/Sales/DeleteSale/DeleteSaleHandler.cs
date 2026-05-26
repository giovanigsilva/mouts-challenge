using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISalesCacheInvalidator _salesCacheInvalidator;
    private readonly ILogger<DeleteSaleHandler> _logger;

    public DeleteSaleHandler(ISaleRepository saleRepository, ISalesCacheInvalidator salesCacheInvalidator, ILogger<DeleteSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _salesCacheInvalidator = salesCacheInvalidator;
        _logger = logger;
    }

    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var deleted = await _saleRepository.DeleteAsync(command.Id, cancellationToken);
        if (!deleted)
        {
            _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "SaleDeleteFailed", "DeleteSale", "NotFound", "Sale", command.Id, DateTime.UtcNow);
            throw new KeyNotFoundException($"Venda com ID {command.Id} nao encontrada.");
        }

        await _salesCacheInvalidator.InvalidateAsync(command.Id, "DeleteSale", cancellationToken);
        _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "SaleDeleted", "DeleteSale", "Success", "Sale", command.Id, DateTime.UtcNow);
        return new DeleteSaleResult { Success = true };
    }
}
