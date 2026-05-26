using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Functions;

public sealed class ItemCancelledFunction : SalesEventFunctionBase
{
    public ItemCancelledFunction(DefaultContext context, ILogger<ItemCancelledFunction> logger) : base(context, logger, nameof(ItemCancelledFunction))
    {
    }

    [Function(nameof(ItemCancelledFunction))]
    public Task Run([ServiceBusTrigger("developerstore.sales.events", "sales-projections-subscription", Connection = "ServiceBusConnection")] string message, CancellationToken cancellationToken)
    {
        return ProcessAsync(message, "ItemCancelled", cancellationToken);
    }
}
