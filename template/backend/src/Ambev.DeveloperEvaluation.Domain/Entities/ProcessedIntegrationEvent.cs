using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class ProcessedIntegrationEvent : BaseEntity
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string ConsumerName { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public Guid? CorrelationId { get; set; }
}
