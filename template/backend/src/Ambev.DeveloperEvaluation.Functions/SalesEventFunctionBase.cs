using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Functions;

public abstract class SalesEventFunctionBase
{
    private readonly DefaultContext _context;
    private readonly ILogger _logger;
    private readonly string _consumerName;

    protected SalesEventFunctionBase(DefaultContext context, ILogger logger, string consumerName)
    {
        _context = context;
        _logger = logger;
        _consumerName = consumerName;
    }

    protected async Task ProcessAsync(string message, string expectedEventType, CancellationToken cancellationToken)
    {
        using var document = JsonDocument.Parse(message);
        var root = document.RootElement;
        var eventId = root.GetProperty("eventId").GetGuid();
        var eventType = root.GetProperty("eventType").GetString();
        var correlationId = root.TryGetProperty("correlationId", out var correlationProperty) && correlationProperty.ValueKind == JsonValueKind.String && Guid.TryParse(correlationProperty.GetString(), out var parsedCorrelationId)
            ? parsedCorrelationId
            : (Guid?)null;

        if (!string.Equals(eventType, expectedEventType, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Evento inesperado. Esperado: {expectedEventType}. Recebido: {eventType}.");

        var alreadyProcessed = await _context.ProcessedIntegrationEvents.AnyAsync(item => item.EventId == eventId && item.ConsumerName == _consumerName, cancellationToken);
        if (alreadyProcessed)
        {
            _logger.LogInformation("Evento duplicado ignorado. EventId: {EventId}, ConsumerName: {ConsumerName}", eventId, _consumerName);
            return;
        }

        _context.ProcessedIntegrationEvents.Add(new ProcessedIntegrationEvent
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            EventType = expectedEventType,
            ConsumerName = _consumerName,
            ProcessedAt = DateTime.UtcNow,
            CorrelationId = correlationId
        });

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Evento processado pela Function. EventId: {EventId}, EventType: {EventType}, ConsumerName: {ConsumerName}", eventId, expectedEventType, _consumerName);
    }
}
