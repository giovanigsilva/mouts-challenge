using System.Text.Json;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

public sealed class GlobalExceptionMiddlewareTests
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger = Substitute.For<ILogger<GlobalExceptionMiddleware>>();

    [Fact(DisplayName = "Given validation exception When middleware handles Then returns 400")]
    public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
    {
        var exception = new ValidationException(new[]
        {
            new ValidationFailure("name", "Nome obrigatorio.")
        });

        var context = await ExecuteAsync(exception);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        await AssertErrorResponseAsync(context, "test-correlation-id");
    }

    [Fact(DisplayName = "Given business rule exception When middleware handles Then returns 400")]
    public async Task InvokeAsync_BusinessRuleException_ReturnsBadRequest()
    {
        var context = await ExecuteAsync(new BusinessRuleException("Regra de negocio violada."));

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        await AssertErrorResponseAsync(context, "test-correlation-id");
    }

    [Fact(DisplayName = "Given key not found exception When middleware handles Then returns 404")]
    public async Task InvokeAsync_KeyNotFoundException_ReturnsNotFound()
    {
        var context = await ExecuteAsync(new KeyNotFoundException("Recurso nao encontrado."));

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        await AssertErrorResponseAsync(context, "test-correlation-id");
    }

    [Fact(DisplayName = "Given operation canceled exception When middleware handles Then does not return 500")]
    public async Task InvokeAsync_OperationCanceledException_DoesNotReturnInternalServerError()
    {
        var context = await ExecuteAsync(new OperationCanceledException());

        context.Response.StatusCode.Should().Be(499);
    }

    [Fact(DisplayName = "Given unhandled exception When middleware handles Then returns 500 with correlation id")]
    public async Task InvokeAsync_Exception_ReturnsInternalServerErrorWithCorrelationId()
    {
        var context = await ExecuteAsync(new Exception("Erro tecnico interno."), "Production");

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        var json = await ReadJsonAsync(context);
        json.RootElement.GetProperty("correlationId").GetString().Should().Be("test-correlation-id");
        json.RootElement.GetProperty("message").GetString().Should().Contain("correlationId");
    }

    private async Task<DefaultHttpContext> ExecuteAsync(Exception exception, string environmentName = "Development")
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Items[CorrelationIdMiddleware.HeaderName] = "test-correlation-id";
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.EnvironmentName.Returns(environmentName);
        var middleware = new GlobalExceptionMiddleware(_ => throw exception, _logger, environment);

        await middleware.InvokeAsync(context);

        return context;
    }

    private static async Task AssertErrorResponseAsync(DefaultHttpContext context, string correlationId)
    {
        var json = await ReadJsonAsync(context);
        json.RootElement.GetProperty("success").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("correlationId").GetString().Should().Be(correlationId);
        json.RootElement.TryGetProperty("timestamp", out _).Should().BeTrue();
    }

    private static async Task<JsonDocument> ReadJsonAsync(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Response.Body.Position = 0;
        return JsonDocument.Parse(body);
    }
}
