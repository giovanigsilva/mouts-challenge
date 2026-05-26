using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public sealed class GetSaleHandler : IRequestHandler<GetSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCacheService _saleCacheService;

    public GetSaleHandler(ISaleRepository saleRepository, ISaleCacheService saleCacheService)
    {
        _saleRepository = saleRepository;
        _saleCacheService = saleCacheService;
    }

    public async Task<SaleResult> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            throw new FluentValidation.ValidationException("Id da venda e obrigatorio.");

        var cachedSale = await _saleCacheService.GetSaleAsync(request.Id, cancellationToken);
        if (cachedSale != null)
            return cachedSale;

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Venda com ID {request.Id} nao encontrada.");

        var result = SaleResultMapper.Map(sale);
        await _saleCacheService.SetSaleAsync(result, cancellationToken);
        return result;
    }
}
