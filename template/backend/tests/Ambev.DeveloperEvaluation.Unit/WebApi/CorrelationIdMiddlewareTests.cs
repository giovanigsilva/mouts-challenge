using Ambev.DeveloperEvaluation.WebApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact(DisplayName = "Given missing correlation header When middleware runs Then generates correlation id")]
    public async Task InvokeAsync_MissingHeader_GeneratesCorrelationId()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CorrelationIdMiddleware(async currentContext => await currentContext.Response.WriteAsync("ok"), Substitute.For<ILogger<CorrelationIdMiddleware>>());

        await middleware.InvokeAsync(context);

        context.Items[CorrelationIdMiddleware.HeaderName].Should().NotBeNull();
        context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "Given correlation header When middleware runs Then preserves correlation id")]
    public async Task InvokeAsync_ExistingHeader_PreservesCorrelationId()
    {
        const string correlationId = "11111111-1111-1111-1111-111111111111";
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;
        var middleware = new CorrelationIdMiddleware(async currentContext => await currentContext.Response.WriteAsync("ok"), Substitute.For<ILogger<CorrelationIdMiddleware>>());

        await middleware.InvokeAsync(context);

        context.Items[CorrelationIdMiddleware.HeaderName].Should().Be(correlationId);
        context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString().Should().Be(correlationId);
    }
}
