using System.Diagnostics;
using System.Security.Claims;
using Serilog.Context;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public sealed class RequestContextLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestContextLoggingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public RequestContextLoggingMiddleware(RequestDelegate next, ILogger<RequestContextLoggingMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value)
            ? value?.ToString()
            : context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();

        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var spanId = Activity.Current?.SpanId.ToString();
        var userId = GetClaimValue(context.User, ClaimTypes.NameIdentifier, "sub", "id") ?? "anonymous";
        var userName = GetClaimValue(context.User, ClaimTypes.Name, "name", "unique_name");
        var userEmail = GetClaimValue(context.User, ClaimTypes.Email, "email");
        var userRole = GetClaimValue(context.User, ClaimTypes.Role, "role");
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();
        var requestPath = context.Request.Path.Value ?? string.Empty;
        var requestMethod = context.Request.Method;
        var routeName = context.GetEndpoint()?.DisplayName;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("SpanId", spanId))
        using (LogContext.PushProperty("Environment", _environment.EnvironmentName))
        using (LogContext.PushProperty("RequestPath", requestPath))
        using (LogContext.PushProperty("RequestMethod", requestMethod))
        using (LogContext.PushProperty("RouteName", routeName))
        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        using (LogContext.PushProperty("RemoteIpAddress", remoteIpAddress))
        using (LogContext.PushProperty("UserAgent", userAgent))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("UserName", userName))
        using (LogContext.PushProperty("UserEmail", userEmail))
        using (LogContext.PushProperty("UserRole", userRole))
        {
            _logger.LogInformation("HTTP request received Method={RequestMethod} Path={RequestPath} CorrelationId={CorrelationId} UserId={UserId} RemoteIpAddress={RemoteIpAddress} UserAgent={UserAgent}", requestMethod, requestPath, correlationId, userId, remoteIpAddress, userAgent);

            await _next(context);

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var resultStatus = statusCode >= 500 ? "Error" : statusCode >= 400 ? "Warning" : "Success";

            using (LogContext.PushProperty("StatusCode", statusCode))
            using (LogContext.PushProperty("ElapsedMilliseconds", stopwatch.ElapsedMilliseconds))
            using (LogContext.PushProperty("ResultStatus", resultStatus))
            {
                if (statusCode >= 500)
                    _logger.LogError("HTTP request finished with server error Method={RequestMethod} Path={RequestPath} StatusCode={StatusCode} ElapsedMilliseconds={ElapsedMilliseconds} CorrelationId={CorrelationId}", requestMethod, requestPath, statusCode, stopwatch.ElapsedMilliseconds, correlationId);
                else if (statusCode >= 400)
                    _logger.LogWarning("HTTP request finished with client error Method={RequestMethod} Path={RequestPath} StatusCode={StatusCode} ElapsedMilliseconds={ElapsedMilliseconds} CorrelationId={CorrelationId}", requestMethod, requestPath, statusCode, stopwatch.ElapsedMilliseconds, correlationId);
                else
                    _logger.LogInformation("HTTP request finished successfully Method={RequestMethod} Path={RequestPath} StatusCode={StatusCode} ElapsedMilliseconds={ElapsedMilliseconds} CorrelationId={CorrelationId}", requestMethod, requestPath, statusCode, stopwatch.ElapsedMilliseconds, correlationId);
            }
        }
    }

    private static string? GetClaimValue(ClaimsPrincipal user, params string[] claimTypes)
    {
        if (user.Identity?.IsAuthenticated != true)
            return null;

        foreach (var claimType in claimTypes)
        {
            var value = user.FindFirst(claimType)?.Value;
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }
}
