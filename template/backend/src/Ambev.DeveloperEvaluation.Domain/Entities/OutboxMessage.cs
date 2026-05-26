using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; } = string.Empty;
    public string Payload { get; set; } = "{}";
    public string Headers { get; set; } = "{}";
    public Guid? CorrelationId { get; set; }
    public Guid? CausationId { get; set; }
    public OutboxMessageStatus Status { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? LastError { get; set; }
    public DateTime? LockedUntil { get; set; }
    public Guid? LockId { get; set; }
    public string? WorkerInstanceId { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public string? DeadLetterReason { get; set; }
}
