using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Common.Behaviors;

public sealed class RequestExceptionLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<RequestExceptionLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestExceptionLoggingBehavior(ILogger<RequestExceptionLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return await next();
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogInformation("UseCase cancelled RequestName={RequestName} OperationName={OperationName} ElapsedMilliseconds={ElapsedMilliseconds} ResultStatus={ResultStatus}", requestName, requestName, stopwatch.ElapsedMilliseconds, "Cancelled");
            throw;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            _logger.LogError(exception, "UseCase failed RequestName={RequestName} OperationName={OperationName} ElapsedMilliseconds={ElapsedMilliseconds} ResultStatus={ResultStatus} ExceptionType={ExceptionType}", requestName, requestName, stopwatch.ElapsedMilliseconds, "Error", exception.GetType().Name);
            throw;
        }
    }
}
