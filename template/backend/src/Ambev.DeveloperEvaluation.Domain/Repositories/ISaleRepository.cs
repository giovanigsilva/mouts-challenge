using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Sale>> ListAsync(string? saleNumber, Guid? customerId, Guid? branchId, bool? isCancelled, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? saleNumber, Guid? customerId, Guid? branchId, bool? isCancelled, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default);
}
