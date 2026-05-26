using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public sealed class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCacheService _saleCacheService;

    public UpdateSaleHandler(ISaleRepository saleRepository, ISaleCacheService saleCacheService)
    {
        _saleRepository = saleRepository;
        _saleCacheService = saleCacheService;
    }

    public async Task<SaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Venda com ID {request.Id} nao encontrada.");

        sale.Update(request.SaleNumber, request.SaleDate, request.CustomerExternalId, request.CustomerName, request.BranchExternalId, request.BranchName);
        sale.ReplaceItems(request.Items.Select(item => new SaleItemInput(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice)));
        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _saleCacheService.InvalidateSaleAsync(sale.Id, cancellationToken);
        await _saleCacheService.InvalidateSalesListAsync(cancellationToken);
        return SaleResultMapper.Map(sale);
    }
}
