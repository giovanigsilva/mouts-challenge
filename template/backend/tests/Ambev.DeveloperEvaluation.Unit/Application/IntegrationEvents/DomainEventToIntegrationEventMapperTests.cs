using Ambev.DeveloperEvaluation.Application.IntegrationEvents;
using Ambev.DeveloperEvaluation.Application.IntegrationEvents.Sales;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.IntegrationEvents;

public sealed class DomainEventToIntegrationEventMapperTests
{
    [Fact(DisplayName = "Given sale created domain event When mapping Then returns versioned integration envelope")]
    public void Map_SaleCreatedEvent_ReturnsIntegrationEnvelope()
    {
        var saleId = Guid.NewGuid();
        var domainEvent = new SaleCreatedEvent(saleId);

        var result = DomainEventToIntegrationEventMapper.Map(domainEvent);

        var envelope = result.Should().BeOfType<IntegrationEventEnvelope<SaleCreatedIntegrationEvent>>().Subject;
        envelope.EventId.Should().Be(domainEvent.EventId);
        envelope.EventType.Should().Be("SaleCreated");
        envelope.SchemaVersion.Should().Be(1);
        envelope.AggregateId.Should().Be(saleId);
        envelope.AggregateType.Should().Be("Sale");
        envelope.Source.Should().Be("DeveloperStore.Sales.Api");
        envelope.Payload.Should().NotBeNull();
    }

    [Fact(DisplayName = "Given item cancelled domain event When mapping Then returns item cancelled payload")]
    public void Map_ItemCancelledEvent_ReturnsItemCancelledPayload()
    {
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var domainEvent = new ItemCancelledEvent(saleId, itemId);

        var result = DomainEventToIntegrationEventMapper.Map(domainEvent);

        var envelope = result.Should().BeOfType<IntegrationEventEnvelope<ItemCancelledIntegrationEvent>>().Subject;
        envelope.EventType.Should().Be("ItemCancelled");
        envelope.Payload.Should().Be(new ItemCancelledIntegrationEvent(saleId, itemId));
    }
}
