using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Policies;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }
    public Guid ProductExternalId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private SaleItem()
    {
    }

    internal SaleItem(Guid productExternalId, string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Update(productExternalId, productName, quantity, unitPrice);
    }

    internal void SetSaleId(Guid saleId)
    {
        SaleId = saleId;
    }

    internal void Update(Guid productExternalId, string productName, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new BusinessRuleException("Item cancelado nao pode ser alterado.");

        if (productExternalId == Guid.Empty)
            throw new BusinessRuleException("Produto e obrigatorio.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new BusinessRuleException("Nome do produto e obrigatorio.");

        if (unitPrice <= 0)
            throw new BusinessRuleException("Preco unitario deve ser maior que zero.");

        ProductExternalId = productExternalId;
        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercentage = SaleDiscountPolicy.GetDiscountPercentage(quantity);
        var grossAmount = quantity * unitPrice;
        DiscountAmount = Math.Round(grossAmount * DiscountPercentage / 100m, 2);
        TotalAmount = Math.Round(grossAmount - DiscountAmount, 2);
        UpdatedAt = DateTime.UtcNow;
    }

    internal void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
