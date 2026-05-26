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
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Venda com ID {command.Id} nao encontrada.");

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale is not null && existingSale.Id != command.Id)
            throw new InvalidOperationException($"Venda com numero {command.SaleNumber} ja existe.");

        var items = command.Items.Select(item => new SaleItem(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice));
        sale.Update(command.SaleNumber, command.SaleDate, command.CustomerExternalId, command.CustomerName, command.BranchExternalId, command.BranchName, items);
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        _logger.LogInformation("SaleModified SaleId={SaleId} SaleNumber={SaleNumber} CustomerExternalId={CustomerExternalId} BranchExternalId={BranchExternalId} TotalAmount={TotalAmount} OccurredAt={OccurredAt}", updatedSale.Id, updatedSale.SaleNumber, updatedSale.CustomerExternalId, updatedSale.BranchExternalId, updatedSale.TotalAmount, DateTime.UtcNow);

        return _mapper.Map<SaleResult>(updatedSale);
    }
}
