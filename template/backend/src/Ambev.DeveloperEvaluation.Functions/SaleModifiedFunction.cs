using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Functions;

public sealed class SaleModifiedFunction : SalesEventFunctionBase
{
    public SaleModifiedFunction(DefaultContext context, ILogger<SaleModifiedFunction> logger) : base(context, logger, nameof(SaleModifiedFunction))
    {
    }

    [Function(nameof(SaleModifiedFunction))]
    public Task Run([ServiceBusTrigger("developerstore.sales.events", "sales-projections-subscription", Connection = "ServiceBusConnection")] string message, CancellationToken cancellationToken)
    {
        return ProcessAsync(message, "SaleModified", cancellationToken);
    }
}
