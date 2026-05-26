using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public sealed class GlobalExceptionMiddleware
{
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value)
            ? value?.ToString()
            : context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();

        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            DomainException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro inesperado ao processar requisicao. CorrelationId: {CorrelationId}", correlationId);
        else
            _logger.LogWarning(exception, "Erro tratado ao processar requisicao. CorrelationId: {CorrelationId}", correlationId);

        var errors = exception is ValidationException validationException
            ? validationException.Errors.Select(error => (ValidationErrorDetail)error)
            : [];

        var message = statusCode == StatusCodes.Status500InternalServerError && !_environment.IsDevelopment()
            ? "Erro interno ao processar a requisicao."
            : exception.Message;

        var response = new
        {
            success = false,
            message,
            errors,
            correlationId,
            timestamp = DateTimeOffset.UtcNow,
            environment = _environment.EnvironmentName
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
