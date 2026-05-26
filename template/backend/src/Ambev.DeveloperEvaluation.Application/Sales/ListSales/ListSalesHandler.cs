using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesQuery, PaginatedSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedSalesResult> Handle(ListSalesQuery query, CancellationToken cancellationToken)
    {
        var validator = new ListSalesValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var result = await _saleRepository.ListAsync(query.Page, query.PageSize, query.SaleNumber, query.CustomerId, query.BranchId, query.IsCancelled, query.FromDate, query.ToDate, cancellationToken);

        return new PaginatedSalesResult
        {
            Items = _mapper.Map<IEnumerable<SaleResult>>(result.Items),
            CurrentPage = query.Page,
            TotalCount = result.TotalCount,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)query.PageSize)
        };
    }
}
