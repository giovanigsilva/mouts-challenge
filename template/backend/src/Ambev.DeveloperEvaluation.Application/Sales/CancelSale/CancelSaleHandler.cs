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
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
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
            throw new KeyNotFoundException($"Venda com ID {command.Id} nao encontrada.");

        sale.Cancel();
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        _logger.LogInformation("SaleCancelled SaleId={SaleId} SaleNumber={SaleNumber} CustomerExternalId={CustomerExternalId} BranchExternalId={BranchExternalId} TotalAmount={TotalAmount} OccurredAt={OccurredAt}", updatedSale.Id, updatedSale.SaleNumber, updatedSale.CustomerExternalId, updatedSale.BranchExternalId, updatedSale.TotalAmount, DateTime.UtcNow);

        return _mapper.Map<SaleResult>(updatedSale);
    }
}
