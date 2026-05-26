using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment environment, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _environment = environment;
        _logger = logger;
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = GetCorrelationId(context);
        var statusCode = GetStatusCode(exception);
        _logger.LogError(exception, "Erro tratado no middleware global. CorrelationId: {CorrelationId}", correlationId);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var response = new ApiResponse
        {
            Success = false,
            Message = GetMessage(exception, statusCode),
            Errors = GetErrors(exception, correlationId)
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            BusinessRuleException => StatusCodes.Status409Conflict,
            InvalidOperationException => StatusCodes.Status409Conflict,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private string GetMessage(Exception exception, int statusCode)
    {
        if (_environment.IsDevelopment())
            return exception.Message;

        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Requisicao invalida.",
            StatusCodes.Status401Unauthorized => "Nao autorizado.",
            StatusCodes.Status404NotFound => "Recurso nao encontrado.",
            StatusCodes.Status409Conflict => exception.Message,
            _ => "Erro interno do servidor."
        };
    }

    private IEnumerable<ValidationErrorDetail> GetErrors(Exception exception, string correlationId)
    {
        if (exception is ValidationException validationException)
            return validationException.Errors.Select(error => (ValidationErrorDetail)error);

        return
        [
            new ValidationErrorDetail
            {
                Error = exception.GetType().Name,
                Detail = $"CorrelationId: {correlationId}"
            }
        ];
    }

    private static string GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var headerValue) && !string.IsNullOrWhiteSpace(headerValue))
            return headerValue.ToString();

        return context.TraceIdentifier;
    }
}
