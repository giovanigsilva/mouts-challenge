namespace Ambev.DeveloperEvaluation.WebApi.Features.Admin.Outbox;

public sealed class OutboxActionRequest
{
    public string Reason { get; set; } = string.Empty;
    public int Limit { get; set; } = 50;
}
