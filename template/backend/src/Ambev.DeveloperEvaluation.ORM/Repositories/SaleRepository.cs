using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
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
}
