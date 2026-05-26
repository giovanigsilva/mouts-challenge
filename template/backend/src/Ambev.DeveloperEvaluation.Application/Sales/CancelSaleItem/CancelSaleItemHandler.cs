using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Venda com ID {command.SaleId} nao encontrada.");

        sale.CancelItem(command.ItemId);
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        var item = updatedSale.Items.First(current => current.Id == command.ItemId);
        try
        {
            _logger.LogInformation("ItemCancelled SaleId={SaleId} SaleNumber={SaleNumber} ItemId={ItemId} ProductExternalId={ProductExternalId} TotalAmount={TotalAmount} OccurredAt={OccurredAt}", updatedSale.Id, updatedSale.SaleNumber, item.Id, item.ProductExternalId, updatedSale.TotalAmount, DateTime.UtcNow);
        }
        catch (Exception logException)
        {
            _logger.LogWarning(logException, "Falha ao registrar evento ItemCancelled em log. SaleId={SaleId} ItemId={ItemId}", updatedSale.Id, item.Id);
        }

        return _mapper.Map<SaleResult>(updatedSale);
    }
}
