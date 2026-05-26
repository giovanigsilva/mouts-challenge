using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed class ItemCancelledEvent
{
    public ItemCancelledEvent(Sale sale, SaleItem item)
    {
        Sale = sale;
        Item = item;
        OccurredAt = DateTime.UtcNow;
    }

    public Sale Sale { get; }
    public SaleItem Item { get; }
    public DateTime OccurredAt { get; }
}
