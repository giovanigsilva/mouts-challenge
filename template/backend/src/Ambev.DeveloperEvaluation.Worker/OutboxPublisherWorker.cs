using Ambev.DeveloperEvaluation.Application.IntegrationEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Worker;

public sealed class OutboxPublisherWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _workerInstanceId = $"{Environment.MachineName}-{Guid.NewGuid()}";

    public OutboxPublisherWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherWorker> logger, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue("Worker:Enabled", true);
        if (!enabled)
        {
            _logger.LogInformation("OutboxPublisherWorker desabilitado por configuracao.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await ProcessBatchAsync(stoppingToken);
                var delaySeconds = processed == 0
                    ? _configuration.GetValue("Worker:EmptyQueueDelaySeconds", 10)
                    : _configuration.GetValue("Worker:PollingIntervalSeconds", 5);

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("OutboxPublisherWorker finalizando com graceful shutdown.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no loop principal do OutboxPublisherWorker.");
                await Task.Delay(TimeSpan.FromSeconds(_configuration.GetValue("Worker:ErrorDelaySeconds", 15)), stoppingToken);
            }
        }
    }

    private async Task<int> ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var publisher = scope.ServiceProvider.GetRequiredService<IEventBusPublisher>();
        var batchSize = _configuration.GetValue("Worker:BatchSize", 10);
        var lockDuration = _configuration.GetValue("Worker:LockDurationSeconds", 60);
        var maxRetries = _configuration.GetValue("Worker:MaxRetries", 5);
        var messages = await repository.GetPendingBatchAsync(batchSize, lockDuration, _workerInstanceId, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(message.Headers) ?? [];
                await publisher.PublishAsync(message.EventType, message.Payload, headers, cancellationToken);
                await repository.MarkPublishedAsync(message.Id, cancellationToken);
                _logger.LogInformation("Mensagem de Outbox publicada. OutboxMessageId: {OutboxMessageId}, EventType: {EventType}", message.Id, message.EventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao publicar mensagem de Outbox. OutboxMessageId: {OutboxMessageId}, EventType: {EventType}", message.Id, message.EventType);
                await repository.MarkFailedAsync(message.Id, ex.Message, maxRetries, cancellationToken);
            }
        }

        return messages.Count;
    }
}
