using Ambev.DeveloperEvaluation.Application.IntegrationEvents;
using Azure.Messaging.ServiceBus;

namespace Ambev.DeveloperEvaluation.Worker.Publishers;

public sealed class AzureServiceBusEventPublisher : IEventBusPublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AzureServiceBusEventPublisher(IConfiguration configuration)
    {
        var connectionString = configuration["ServiceBus:ConnectionString"];
        var topicName = configuration["ServiceBus:TopicName"] ?? configuration["ServiceBus:QueueName"] ?? "developerstore.sales.events";
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(topicName);
    }

    public async Task PublishAsync(string eventType, string payload, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken)
    {
        var message = new ServiceBusMessage(payload)
        {
            ContentType = "application/json",
            Subject = eventType,
            MessageId = Guid.NewGuid().ToString()
        };

        foreach (var header in headers)
        {
            message.ApplicationProperties[header.Key] = header.Value;
        }

        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
