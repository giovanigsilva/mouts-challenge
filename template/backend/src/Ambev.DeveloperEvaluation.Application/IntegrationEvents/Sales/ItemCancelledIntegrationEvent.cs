namespace Ambev.DeveloperEvaluation.Application.IntegrationEvents.Sales;

public sealed record ItemCancelledIntegrationEvent(Guid SaleId, Guid ItemId);
