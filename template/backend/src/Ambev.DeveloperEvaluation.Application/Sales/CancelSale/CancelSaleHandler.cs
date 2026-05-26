using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCacheService _saleCacheService;

    public CancelSaleHandler(ISaleRepository saleRepository, ISaleCacheService saleCacheService)
    {
        _saleRepository = saleRepository;
        _saleCacheService = saleCacheService;
    }

    public async Task<SaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Venda com ID {request.Id} nao encontrada.");

        sale.Cancel();
        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _saleCacheService.InvalidateSaleAsync(sale.Id, cancellationToken);
        await _saleCacheService.InvalidateSalesListAsync(cancellationToken);
        return SaleResultMapper.Map(sale);
    }
}
