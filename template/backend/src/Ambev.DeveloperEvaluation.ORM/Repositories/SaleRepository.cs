using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<(IEnumerable<Sale> Items, int TotalCount)> ListAsync(int page, int pageSize, string? saleNumber, Guid? customerId, Guid? branchId, bool? isCancelled, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(sale => sale.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(saleNumber))
            query = query.Where(sale => sale.SaleNumber == saleNumber);

        if (customerId.HasValue)
            query = query.Where(sale => sale.CustomerExternalId == customerId.Value);

        if (branchId.HasValue)
            query = query.Where(sale => sale.BranchExternalId == branchId.Value);

        if (isCancelled.HasValue)
            query = query.Where(sale => sale.IsCancelled == isCancelled.Value);

        if (fromDate.HasValue)
            query = query.Where(sale => sale.SaleDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(sale => sale.SaleDate <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(sale => sale.SaleDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale is null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.AnyAsync(sale => sale.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SalesByUserReport>> GetSalesByUserReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var salesQuery = _context.Sales.AsNoTracking();

        if (fromDate.HasValue)
            salesQuery = salesQuery.Where(sale => sale.SaleDate >= fromDate.Value);

        if (toDate.HasValue)
            salesQuery = salesQuery.Where(sale => sale.SaleDate <= toDate.Value);

        return await _context.Users
            .AsNoTracking()
            .GroupJoin(
                salesQuery,
                user => user.Id,
                sale => sale.CreatedByUserId,
                (user, sales) => new SalesByUserReport
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    TotalSales = sales.Count(),
                    ActiveSales = sales.Count(sale => !sale.IsCancelled),
                    CancelledSales = sales.Count(sale => sale.IsCancelled),
                    TotalSoldAmount = sales.Where(sale => !sale.IsCancelled).Sum(sale => sale.TotalAmount),
                    FirstSaleDate = sales.Min(sale => (DateTime?)sale.SaleDate),
                    LastSaleDate = sales.Max(sale => (DateTime?)sale.SaleDate)
                })
            .OrderByDescending(item => item.TotalSoldAmount)
            .ThenBy(item => item.Username)
            .ToListAsync(cancellationToken);
    }
}
