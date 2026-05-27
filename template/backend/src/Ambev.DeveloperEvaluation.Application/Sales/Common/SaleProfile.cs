using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Reports;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Reports;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SaleItemInput, SaleItem>();
        CreateMap<CreateSaleCommand, Sale>();
        CreateMap<UpdateSaleCommand, Sale>();
        CreateMap<Sale, SaleResult>();
        CreateMap<SaleItem, SaleItemResult>();
        CreateMap<SalesByUserReport, SalesByUserReportResult>();
    }
}
