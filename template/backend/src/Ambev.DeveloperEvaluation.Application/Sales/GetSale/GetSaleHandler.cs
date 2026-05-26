using Ambev.DeveloperEvaluation.Application.Common.Caching;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleQuery, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleHandler> _logger;

    public GetSaleHandler(ISaleRepository saleRepository, ICacheService cacheService, IMapper mapper, ILogger<GetSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(GetSaleQuery query, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await _cacheService.GetOrCreateAsync(
            SalesCacheKeys.Details(query.Id),
            SalesCacheKeys.DetailsKeyType,
            async token =>
            {
                var sale = await _saleRepository.GetByIdAsync(query.Id, token);
                if (sale is null)
                {
                    _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "SaleGetFailed", "GetSale", "NotFound", "Sale", query.Id, DateTime.UtcNow);
                    throw new KeyNotFoundException($"Venda com ID {query.Id} nao encontrada.");
                }

                _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} SaleId={SaleId} SaleNumber={SaleNumber} OccurredAtUtc={OccurredAtUtc}", "SaleRetrieved", "GetSale", "Success", "Sale", sale.Id, sale.Id, sale.SaleNumber, DateTime.UtcNow);
                return _mapper.Map<SaleResult>(sale);
            },
            cancellationToken);
    }
}
