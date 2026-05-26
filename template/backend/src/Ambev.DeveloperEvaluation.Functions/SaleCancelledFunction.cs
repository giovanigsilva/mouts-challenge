using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Functions;

public sealed class SaleCancelledFunction : SalesEventFunctionBase
{
    public SaleCancelledFunction(DefaultContext context, ILogger<SaleCancelledFunction> logger) : base(context, logger, nameof(SaleCancelledFunction))
    {
    }

    [Function(nameof(SaleCancelledFunction))]
    public Task Run([ServiceBusTrigger("developerstore.sales.events", "sales-notifications-subscription", Connection = "ServiceBusConnection")] string message, CancellationToken cancellationToken)
    {
        return ProcessAsync(message, "SaleCancelled", cancellationToken);
    }
}
