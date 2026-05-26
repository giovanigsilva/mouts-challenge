using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IOutboxRepository
{
    Task<IReadOnlyCollection<OutboxMessage>> ListAsync(OutboxMessageStatus? status, string? eventType, Guid? aggregateId, Guid? correlationId, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken);
    Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<OutboxMessage>> GetPendingBatchAsync(int batchSize, int lockDurationSeconds, string workerInstanceId, CancellationToken cancellationToken);
    Task MarkPublishedAsync(Guid id, CancellationToken cancellationToken);
    Task MarkFailedAsync(Guid id, string error, int maxRetries, CancellationToken cancellationToken);
    Task RetryAsync(Guid id, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken);
    Task DeadLetterAsync(Guid id, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken);
    Task ResetAsync(Guid id, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken);
    Task<int> RetryByStatusAsync(OutboxMessageStatus status, int limit, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken);
    Task<OutboxStats> GetStatsAsync(CancellationToken cancellationToken);
}
