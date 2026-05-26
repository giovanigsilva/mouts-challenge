using Ambev.DeveloperEvaluation.Application.IntegrationEvents;

namespace Ambev.DeveloperEvaluation.Worker.Publishers;

public sealed class LogOnlyEventBusPublisher : IEventBusPublisher
{
    private readonly ILogger<LogOnlyEventBusPublisher> _logger;

    public LogOnlyEventBusPublisher(ILogger<LogOnlyEventBusPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string eventType, string payload, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publicacao simulada de evento de integracao. EventType: {EventType}, PayloadLength: {PayloadLength}", eventType, payload.Length);
        return Task.CompletedTask;
    }
}
