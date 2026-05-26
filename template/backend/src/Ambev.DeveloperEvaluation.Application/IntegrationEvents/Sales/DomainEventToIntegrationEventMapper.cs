using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.IntegrationEvents.Sales;

public static class DomainEventToIntegrationEventMapper
{
    public static string GetEventType(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            SaleCreatedEvent => "SaleCreated",
            SaleModifiedEvent => "SaleModified",
            SaleCancelledEvent => "SaleCancelled",
            ItemCancelledEvent => "ItemCancelled",
            _ => domainEvent.GetType().Name.Replace("Event", string.Empty)
        };
    }

    public static object Map(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            SaleCreatedEvent saleCreated => CreateEnvelope(saleCreated, "SaleCreated", new SaleCreatedIntegrationEvent(saleCreated.AggregateId)),
            SaleModifiedEvent saleModified => CreateEnvelope(saleModified, "SaleModified", new SaleModifiedIntegrationEvent(saleModified.AggregateId)),
            SaleCancelledEvent saleCancelled => CreateEnvelope(saleCancelled, "SaleCancelled", new SaleCancelledIntegrationEvent(saleCancelled.AggregateId)),
            ItemCancelledEvent itemCancelled => CreateEnvelope(itemCancelled, "ItemCancelled", new ItemCancelledIntegrationEvent(itemCancelled.AggregateId, itemCancelled.ItemId)),
            _ => CreateEnvelope(domainEvent, domainEvent.GetType().Name.Replace("Event", string.Empty), new { domainEvent.AggregateId })
        };
    }

    private static IntegrationEventEnvelope<TPayload> CreateEnvelope<TPayload>(IDomainEvent domainEvent, string eventType, TPayload payload)
    {
        return new IntegrationEventEnvelope<TPayload>
        {
            EventId = domainEvent.EventId,
            EventType = eventType,
            SchemaVersion = 1,
            OccurredAt = domainEvent.OccurredAt,
            AggregateId = domainEvent.AggregateId,
            AggregateType = "Sale",
            Source = "DeveloperStore.Sales.Api",
            Payload = payload
        };
    }
}
