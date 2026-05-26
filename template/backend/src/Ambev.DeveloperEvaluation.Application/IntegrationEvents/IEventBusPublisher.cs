namespace Ambev.DeveloperEvaluation.Application.IntegrationEvents;

public interface IEventBusPublisher
{
    Task PublishAsync(string eventType, string payload, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken);
}
