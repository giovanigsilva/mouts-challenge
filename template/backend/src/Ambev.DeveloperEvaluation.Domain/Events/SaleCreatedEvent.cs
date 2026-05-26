namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed class SaleCreatedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public SaleCreatedEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }
}
