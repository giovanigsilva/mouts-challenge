using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed class SaleModifiedEvent
{
    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale;
        OccurredAt = DateTime.UtcNow;
    }

    public Sale Sale { get; }
    public DateTime OccurredAt { get; }
}
