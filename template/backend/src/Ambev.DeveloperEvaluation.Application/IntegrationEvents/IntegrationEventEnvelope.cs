namespace Ambev.DeveloperEvaluation.Application.IntegrationEvents;

public sealed class IntegrationEventEnvelope<TPayload>
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int SchemaVersion { get; set; } = 1;
    public DateTime OccurredAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid? CorrelationId { get; set; }
    public Guid? CausationId { get; set; }
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public TPayload? Payload { get; set; }
}
