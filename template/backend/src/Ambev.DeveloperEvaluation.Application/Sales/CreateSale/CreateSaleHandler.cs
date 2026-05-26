using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public sealed class CreateSaleHandler : IRequestHandler<CreateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCacheService _saleCacheService;

    public CreateSaleHandler(ISaleRepository saleRepository, ISaleCacheService saleCacheService)
    {
        _saleRepository = saleRepository;
        _saleCacheService = saleCacheService;
    }

    public async Task<SaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Venda com numero {command.SaleNumber} ja existe.");

        var sale = new Sale(command.SaleNumber, command.SaleDate, command.CustomerExternalId, command.CustomerName, command.BranchExternalId, command.BranchName);

        foreach (var item in command.Items)
        {
            sale.AddItem(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        await _saleCacheService.InvalidateSalesListAsync(cancellationToken);
        return SaleResultMapper.Map(createdSale);
    }
}
