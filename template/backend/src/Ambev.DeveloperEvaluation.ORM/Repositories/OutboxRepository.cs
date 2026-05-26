using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly DefaultContext _context;

    public OutboxRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<OutboxMessage>> ListAsync(OutboxMessageStatus? status, string? eventType, Guid? aggregateId, Guid? correlationId, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken)
    {
        return await ApplyFilters(_context.OutboxMessages.AsNoTracking(), status, eventType, aggregateId, correlationId, fromDate, toDate)
            .OrderByDescending(message => message.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.OutboxMessages.AsNoTracking().FirstOrDefaultAsync(message => message.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<OutboxMessage>> GetPendingBatchAsync(int batchSize, int lockDurationSeconds, string workerInstanceId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var messages = await _context.OutboxMessages
            .Where(message => message.Status == OutboxMessageStatus.Pending && (message.NextRetryAt == null || message.NextRetryAt <= now))
            .OrderBy(message => message.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            message.Status = OutboxMessageStatus.Processing;
            message.LockId = Guid.NewGuid();
            message.LockedUntil = now.AddSeconds(lockDurationSeconds);
            message.WorkerInstanceId = workerInstanceId;
            message.LastAttemptAt = now;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return messages;
    }

    public async Task MarkPublishedAsync(Guid id, CancellationToken cancellationToken)
    {
        var message = await GetTrackedMessageAsync(id, cancellationToken);
        message.Status = OutboxMessageStatus.Published;
        message.PublishedAt = DateTime.UtcNow;
        message.LockId = null;
        message.LockedUntil = null;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(Guid id, string error, int maxRetries, CancellationToken cancellationToken)
    {
        var message = await GetTrackedMessageAsync(id, cancellationToken);
        message.RetryCount++;
        message.LastError = error;
        message.LastAttemptAt = DateTime.UtcNow;
        message.LockId = null;
        message.LockedUntil = null;

        if (message.RetryCount >= maxRetries)
        {
            message.Status = OutboxMessageStatus.DeadLettered;
            message.DeadLetterReason = error;
        }
        else
        {
            message.Status = OutboxMessageStatus.Failed;
            message.NextRetryAt = DateTime.UtcNow.AddSeconds(Math.Min(300, message.RetryCount * 30));
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RetryAsync(Guid id, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken)
    {
        var message = await GetTrackedMessageAsync(id, cancellationToken);
        var previousStatus = message.Status.ToString();
        message.Status = OutboxMessageStatus.Pending;
        message.NextRetryAt = DateTime.UtcNow;
        message.LockId = null;
        message.LockedUntil = null;
        message.DeadLetterReason = null;
        AddAdminAction(id, "Retry", reason, performedBy, correlationId, previousStatus, message.Status.ToString());
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeadLetterAsync(Guid id, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken)
    {
        var message = await GetTrackedMessageAsync(id, cancellationToken);
        var previousStatus = message.Status.ToString();
        message.Status = OutboxMessageStatus.DeadLettered;
        message.DeadLetterReason = reason;
        message.LockId = null;
        message.LockedUntil = null;
        AddAdminAction(id, "DeadLetter", reason, performedBy, correlationId, previousStatus, message.Status.ToString());
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetAsync(Guid id, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken)
    {
        var message = await GetTrackedMessageAsync(id, cancellationToken);
        var previousStatus = message.Status.ToString();
        message.Status = OutboxMessageStatus.Pending;
        message.LockId = null;
        message.LockedUntil = null;
        message.WorkerInstanceId = null;
        AddAdminAction(id, "Reset", reason, performedBy, correlationId, previousStatus, message.Status.ToString());
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> RetryByStatusAsync(OutboxMessageStatus status, int limit, string reason, string performedBy, Guid? correlationId, CancellationToken cancellationToken)
    {
        var messages = await _context.OutboxMessages
            .Where(message => message.Status == status)
            .OrderBy(message => message.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            var previousStatus = message.Status.ToString();
            message.Status = OutboxMessageStatus.Pending;
            message.NextRetryAt = DateTime.UtcNow;
            message.LockId = null;
            message.LockedUntil = null;
            message.DeadLetterReason = null;
            AddAdminAction(message.Id, $"Retry{status}", reason, performedBy, correlationId, previousStatus, message.Status.ToString());
        }

        await _context.SaveChangesAsync(cancellationToken);
        return messages.Count;
    }

    public async Task<OutboxStats> GetStatsAsync(CancellationToken cancellationToken)
    {
        var messages = await _context.OutboxMessages.AsNoTracking().ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var published = messages.Where(message => message.Status == OutboxMessageStatus.Published).ToList();
        var failed = messages.Where(message => message.Status is OutboxMessageStatus.Failed or OutboxMessageStatus.DeadLettered).ToList();

        return new OutboxStats
        {
            PendingCount = messages.Count(message => message.Status == OutboxMessageStatus.Pending),
            ProcessingCount = messages.Count(message => message.Status == OutboxMessageStatus.Processing),
            PublishedCount = published.Count,
            FailedCount = messages.Count(message => message.Status == OutboxMessageStatus.Failed),
            DeadLetteredCount = messages.Count(message => message.Status == OutboxMessageStatus.DeadLettered),
            OldestPendingMessageAgeSeconds = messages.Where(message => message.Status == OutboxMessageStatus.Pending).OrderBy(message => message.CreatedAt).Select(message => (double?)(now - message.CreatedAt).TotalSeconds).FirstOrDefault(),
            LastPublishedAt = published.OrderByDescending(message => message.PublishedAt).Select(message => message.PublishedAt).FirstOrDefault(),
            LastFailedAt = failed.OrderByDescending(message => message.LastAttemptAt).Select(message => message.LastAttemptAt).FirstOrDefault(),
            RetryQueueSize = messages.Count(message => message.Status == OutboxMessageStatus.Failed && (message.NextRetryAt == null || message.NextRetryAt <= now)),
            AveragePublishLatencyMs = published.Where(message => message.PublishedAt.HasValue).Select(message => (double?)(message.PublishedAt!.Value - message.CreatedAt).TotalMilliseconds).Average(),
            ErrorRate = messages.Count == 0 ? 0 : failed.Count / (double)messages.Count
        };
    }

    private static IQueryable<OutboxMessage> ApplyFilters(IQueryable<OutboxMessage> query, OutboxMessageStatus? status, string? eventType, Guid? aggregateId, Guid? correlationId, DateTime? fromDate, DateTime? toDate)
    {
        if (status.HasValue)
            query = query.Where(message => message.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(message => message.EventType == eventType);

        if (aggregateId.HasValue)
            query = query.Where(message => message.AggregateId == aggregateId.Value);

        if (correlationId.HasValue)
            query = query.Where(message => message.CorrelationId == correlationId.Value);

        if (fromDate.HasValue)
            query = query.Where(message => message.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(message => message.CreatedAt <= toDate.Value);

        return query;
    }

    private async Task<OutboxMessage> GetTrackedMessageAsync(Guid id, CancellationToken cancellationToken)
    {
        var message = await _context.OutboxMessages.FirstOrDefaultAsync(current => current.Id == id, cancellationToken);
        if (message == null)
            throw new KeyNotFoundException($"Mensagem de Outbox {id} nao encontrada.");

        return message;
    }

    private void AddAdminAction(Guid? outboxMessageId, string action, string reason, string performedBy, Guid? correlationId, string? previousStatus, string? newStatus)
    {
        _context.OutboxAdminActions.Add(new OutboxAdminAction
        {
            Id = Guid.NewGuid(),
            OutboxMessageId = outboxMessageId,
            Action = action,
            Reason = reason,
            PerformedBy = performedBy,
            PerformedAt = DateTime.UtcNow,
            CorrelationId = correlationId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus
        });
    }
}
