namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed class SaleCancelledEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public SaleCancelledEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }
}
