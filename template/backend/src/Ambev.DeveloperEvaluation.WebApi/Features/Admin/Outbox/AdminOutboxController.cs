using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Admin.Outbox;

[ApiController]
[Authorize(Policy = "Queue.Admin")]
[Tags("Fila e Reprocessamento")]
[Route("api/admin/outbox")]
public sealed class AdminOutboxController : ControllerBase
{
    private const int MaxPageSize = 100;
    private const int MaxBatchLimit = 500;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<AdminOutboxController> _logger;

    public AdminOutboxController(IOutboxRepository outboxRepository, ILogger<AdminOutboxController> logger)
    {
        _outboxRepository = outboxRepository;
        _logger = logger;
    }

    [HttpGet("messages")]
    [ProducesResponseType(typeof(ApiResponseWithData<IReadOnlyCollection<OutboxMessageResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] OutboxMessageStatus? status, [FromQuery] string? eventType, [FromQuery] Guid? aggregateId, [FromQuery] Guid? correlationId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var currentPage = page <= 0 ? 1 : page;
        var currentPageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, MaxPageSize);
        var messages = await _outboxRepository.ListAsync(status, eventType, aggregateId, correlationId, fromDate, toDate, currentPage, currentPageSize, cancellationToken);

        return Ok(new ApiResponseWithData<IReadOnlyCollection<OutboxMessageResponse>>
        {
            Success = true,
            Message = "Mensagens de Outbox recuperadas com sucesso.",
            Data = messages.Select(Map).ToList()
        });
    }

    [HttpGet("messages/{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<OutboxMessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var message = await _outboxRepository.GetByIdAsync(id, cancellationToken);
        if (message == null)
            throw new KeyNotFoundException($"Mensagem de Outbox {id} nao encontrada.");

        return Ok(new ApiResponseWithData<OutboxMessageResponse>
        {
            Success = true,
            Message = "Mensagem de Outbox recuperada com sucesso.",
            Data = Map(message)
        });
    }

    [HttpPost("messages/{id}/retry")]
    public async Task<IActionResult> Retry([FromRoute] Guid id, [FromBody] OutboxActionRequest request, CancellationToken cancellationToken)
    {
        await _outboxRepository.RetryAsync(id, request.Reason, GetUser(), GetCorrelationId(), cancellationToken);
        LogAdminAction("Retry", id, request.Reason);
        return Ok(new ApiResponse { Success = true, Message = "Mensagem reenfileirada com sucesso." });
    }

    [HttpPost("messages/{id}/deadletter")]
    public async Task<IActionResult> DeadLetter([FromRoute] Guid id, [FromBody] OutboxActionRequest request, CancellationToken cancellationToken)
    {
        await _outboxRepository.DeadLetterAsync(id, request.Reason, GetUser(), GetCorrelationId(), cancellationToken);
        LogAdminAction("DeadLetter", id, request.Reason);
        return Ok(new ApiResponse { Success = true, Message = "Mensagem marcada como DeadLettered com sucesso." });
    }

    [HttpPost("messages/{id}/reset")]
    public async Task<IActionResult> Reset([FromRoute] Guid id, [FromBody] OutboxActionRequest request, CancellationToken cancellationToken)
    {
        await _outboxRepository.ResetAsync(id, request.Reason, GetUser(), GetCorrelationId(), cancellationToken);
        LogAdminAction("Reset", id, request.Reason);
        return Ok(new ApiResponse { Success = true, Message = "Lock da mensagem resetado com sucesso." });
    }

    [HttpPost("retry-failed")]
    public async Task<IActionResult> RetryFailed([FromBody] OutboxActionRequest request, CancellationToken cancellationToken)
    {
        var count = await _outboxRepository.RetryByStatusAsync(OutboxMessageStatus.Failed, NormalizeLimit(request.Limit), request.Reason, GetUser(), GetCorrelationId(), cancellationToken);
        return Ok(new ApiResponseWithData<object> { Success = true, Message = "Mensagens Failed reenfileiradas com sucesso.", Data = new { count } });
    }

    [HttpPost("retry-deadlettered")]
    public async Task<IActionResult> RetryDeadLettered([FromBody] OutboxActionRequest request, CancellationToken cancellationToken)
    {
        var count = await _outboxRepository.RetryByStatusAsync(OutboxMessageStatus.DeadLettered, NormalizeLimit(request.Limit), request.Reason, GetUser(), GetCorrelationId(), cancellationToken);
        return Ok(new ApiResponseWithData<object> { Success = true, Message = "Mensagens DeadLettered reenfileiradas com sucesso.", Data = new { count } });
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponseWithData<OutboxStats>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Stats(CancellationToken cancellationToken)
    {
        var stats = await _outboxRepository.GetStatsAsync(cancellationToken);
        return Ok(new ApiResponseWithData<OutboxStats> { Success = true, Message = "Estatisticas da Outbox recuperadas com sucesso.", Data = stats });
    }

    private static OutboxMessageResponse Map(OutboxMessage message)
    {
        return new OutboxMessageResponse
        {
            Id = message.Id,
            EventId = message.EventId,
            EventType = message.EventType,
            AggregateId = message.AggregateId,
            AggregateType = message.AggregateType,
            Status = message.Status,
            RetryCount = message.RetryCount,
            MaxRetries = message.MaxRetries,
            CreatedAt = message.CreatedAt,
            PublishedAt = message.PublishedAt,
            NextRetryAt = message.NextRetryAt,
            LastError = message.LastError,
            DeadLetterReason = message.DeadLetterReason,
            CorrelationId = message.CorrelationId
        };
    }

    private string GetUser()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "unknown";
    }

    private Guid? GetCorrelationId()
    {
        return Guid.TryParse(HttpContext.TraceIdentifier, out var correlationId) ? correlationId : null;
    }

    private int NormalizeLimit(int limit)
    {
        return limit <= 0 ? 50 : Math.Min(limit, MaxBatchLimit);
    }

    private void LogAdminAction(string action, Guid outboxMessageId, string reason)
    {
        _logger.LogInformation("Acao administrativa de Outbox executada. Action: {Action}, OutboxMessageId: {OutboxMessageId}, Reason: {Reason}, User: {User}, CorrelationId: {CorrelationId}", action, outboxMessageId, reason, GetUser(), HttpContext.TraceIdentifier);
    }
}
