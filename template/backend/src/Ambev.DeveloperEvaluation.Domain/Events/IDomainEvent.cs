namespace Ambev.DeveloperEvaluation.Domain.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    Guid AggregateId { get; }
    DateTime OccurredAt { get; }
}
