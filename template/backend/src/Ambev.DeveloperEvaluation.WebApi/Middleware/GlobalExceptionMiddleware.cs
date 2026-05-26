using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private const int ClientClosedRequestStatusCode = 499;

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException exception) when (context.RequestAborted.IsCancellationRequested)
        {
            await HandleExceptionAsync(context, exception);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning(exception, "Nao foi possivel escrever resposta de erro porque a resposta ja foi iniciada. Method: {Method} Path: {Path}", context.Request.Method, context.Request.Path);
            return;
        }

        var correlationId = context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value)
            ? value?.ToString()
            : context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();

        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            DomainException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            OperationCanceledException => ClientClosedRequestStatusCode,
            TimeoutException => StatusCodes.Status408RequestTimeout,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            DbUpdateException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        if (exception is OperationCanceledException)
            _logger.LogInformation(exception, "Requisicao cancelada pelo cliente. CorrelationId: {CorrelationId} Method: {Method} Path: {Path} StatusCode: {StatusCode} UserId: {UserId}", correlationId, context.Request.Method, context.Request.Path, statusCode, context.User?.Identity?.Name);
        else if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro inesperado ao processar requisicao. CorrelationId: {CorrelationId} Method: {Method} Path: {Path} StatusCode: {StatusCode} UserId: {UserId}", correlationId, context.Request.Method, context.Request.Path, statusCode, context.User?.Identity?.Name);
        else
            _logger.LogWarning(exception, "Erro tratado ao processar requisicao. CorrelationId: {CorrelationId} Method: {Method} Path: {Path} StatusCode: {StatusCode} UserId: {UserId}", correlationId, context.Request.Method, context.Request.Path, statusCode, context.User?.Identity?.Name);

        var errors = exception is ValidationException validationException
            ? validationException.Errors.Select(error => (ValidationErrorDetail)error)
            : [];

        var message = GetSafeMessage(exception, statusCode);

        var response = new
        {
            success = false,
            message,
            errors,
            correlationId,
            timestamp = DateTimeOffset.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        try
        {
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }), context.RequestAborted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cliente desconectou antes de receber a resposta de erro. CorrelationId: {CorrelationId} Method: {Method} Path: {Path}", correlationId, context.Request.Method, context.Request.Path);
        }
    }

    private string GetSafeMessage(Exception exception, int statusCode)
    {
        if (exception is OperationCanceledException)
            return "A requisicao foi cancelada pelo cliente.";

        if (exception is TimeoutException)
            return "A requisicao excedeu o tempo limite de processamento.";

        if (exception is DbUpdateConcurrencyException)
            return "A operacao nao pode ser concluida porque o recurso foi alterado por outro processo.";

        if (exception is DbUpdateException)
            return "Erro ao persistir dados. Informe o correlationId ao suporte.";

        if (statusCode == StatusCodes.Status500InternalServerError && !_environment.IsDevelopment())
            return "Erro interno ao processar a requisicao. Informe o correlationId ao suporte.";

        return exception.Message;
    }
}
