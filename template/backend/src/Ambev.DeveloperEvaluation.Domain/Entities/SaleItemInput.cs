namespace Ambev.DeveloperEvaluation.Domain.Entities;

public sealed record SaleItemInput(Guid ProductExternalId, string ProductName, int Quantity, decimal UnitPrice);
