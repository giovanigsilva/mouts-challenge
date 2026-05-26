using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public sealed class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCacheService _saleCacheService;

    public DeleteSaleHandler(ISaleRepository saleRepository, ISaleCacheService saleCacheService)
    {
        _saleRepository = saleRepository;
        _saleCacheService = saleCacheService;
    }

    public async Task Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Venda com ID {request.Id} nao encontrada.");

        await _saleRepository.DeleteAsync(sale, cancellationToken);
        await _saleCacheService.InvalidateSaleAsync(sale.Id, cancellationToken);
        await _saleCacheService.InvalidateSalesListAsync(cancellationToken);
    }
}
