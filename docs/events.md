# Events

Domain events:

- `SaleCreatedEvent`
- `SaleModifiedEvent`
- `SaleCancelledEvent`
- `ItemCancelledEvent`

Integration events:

- `SaleCreatedIntegrationEvent`
- `SaleModifiedIntegrationEvent`
- `SaleCancelledIntegrationEvent`
- `ItemCancelledIntegrationEvent`

Envelope:

```json
{
  "eventId": "guid",
  "eventType": "SaleCreated",
  "schemaVersion": 1,
  "occurredAt": "2026-05-26T12:00:00Z",
  "publishedAt": null,
  "correlationId": null,
  "causationId": null,
  "aggregateId": "guid",
  "aggregateType": "Sale",
  "source": "DeveloperStore.Sales.Api",
  "payload": {}
}
```

Flow:

1. API receives the request.
2. Application handler calls the Sale aggregate.
3. Domain raises a domain event.
4. Repository persists Sale and Outbox message.
5. Worker reads pending Outbox messages.
6. Worker publishes to Azure Service Bus when configured.
7. In local simulation, Worker uses `LogOnlyEventBusPublisher`.
8. Functions consume Service Bus messages and record processed events.

Service Bus target:

- Topic: `developerstore.sales.events`
- Subscriptions:
  - `sales-audit-subscription`
  - `sales-notifications-subscription`
  - `sales-projections-subscription`

