using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Common.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("UseCase started RequestName={RequestName} OperationName={OperationName} ResultStatus={ResultStatus}", requestName, requestName, "Started");

        var response = await next();

        stopwatch.Stop();
        _logger.LogInformation("UseCase completed RequestName={RequestName} OperationName={OperationName} ElapsedMilliseconds={ElapsedMilliseconds} ResultStatus={ResultStatus}", requestName, requestName, stopwatch.ElapsedMilliseconds, "Success");

        return response;
    }
}
