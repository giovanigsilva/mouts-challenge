namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public sealed class OutboxStats
{
    public int PendingCount { get; set; }
    public int ProcessingCount { get; set; }
    public int PublishedCount { get; set; }
    public int FailedCount { get; set; }
    public int DeadLetteredCount { get; set; }
    public double? OldestPendingMessageAgeSeconds { get; set; }
    public DateTime? LastPublishedAt { get; set; }
    public DateTime? LastFailedAt { get; set; }
    public int RetryQueueSize { get; set; }
    public double? AveragePublishLatencyMs { get; set; }
    public double ErrorRate { get; set; }
}
