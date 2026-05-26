namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed class SaleModifiedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public SaleModifiedEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }
}
