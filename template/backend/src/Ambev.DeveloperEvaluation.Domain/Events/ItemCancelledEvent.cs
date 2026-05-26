namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed class ItemCancelledEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId { get; }
    public Guid ItemId { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public ItemCancelledEvent(Guid aggregateId, Guid itemId)
    {
        AggregateId = aggregateId;
        ItemId = itemId;
    }
}
