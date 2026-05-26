using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Functions;

public sealed class SaleCreatedFunction : SalesEventFunctionBase
{
    public SaleCreatedFunction(DefaultContext context, ILogger<SaleCreatedFunction> logger) : base(context, logger, nameof(SaleCreatedFunction))
    {
    }

    [Function(nameof(SaleCreatedFunction))]
    public Task Run([ServiceBusTrigger("developerstore.sales.events", "sales-audit-subscription", Connection = "ServiceBusConnection")] string message, CancellationToken cancellationToken)
    {
        return ProcessAsync(message, "SaleCreated", cancellationToken);
    }
}
