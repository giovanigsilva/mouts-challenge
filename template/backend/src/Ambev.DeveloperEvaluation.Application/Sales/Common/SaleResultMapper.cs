using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public static class SaleResultMapper
{
    public static SaleResult Map(Sale sale)
    {
        return new SaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerExternalId = sale.CustomerExternalId,
            CustomerName = sale.CustomerName,
            BranchExternalId = sale.BranchExternalId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(MapItem).ToList()
        };
    }

    private static SaleItemResult MapItem(SaleItem item)
    {
        return new SaleItemResult
        {
            Id = item.Id,
            ProductExternalId = item.ProductExternalId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            DiscountPercentage = item.DiscountPercentage,
            DiscountAmount = item.DiscountAmount,
            TotalAmount = item.TotalAmount,
            IsCancelled = item.IsCancelled
        };
    }
}
