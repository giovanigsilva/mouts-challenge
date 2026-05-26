using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class OutboxAdminAction : BaseEntity
{
    public Guid? OutboxMessageId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public Guid? CorrelationId { get; set; }
    public string? PreviousStatus { get; set; }
    public string? NewStatus { get; set; }
}
