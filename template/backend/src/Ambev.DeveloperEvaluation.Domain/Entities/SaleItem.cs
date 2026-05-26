using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Services;

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
    private SaleItem()
    {
    }

    public SaleItem(Guid productExternalId, string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        SetProduct(productExternalId, productName);
        Update(quantity, unitPrice);
    }

    public void Update(int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new BusinessRuleException("Item cancelado nao pode ser alterado.");

        if (unitPrice <= 0)
            throw new BusinessRuleException("O preco unitario deve ser maior que zero.");

        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercentage = SaleDiscountPolicy.GetDiscountPercentage(quantity);

        var grossAmount = Quantity * UnitPrice;
        DiscountAmount = grossAmount * DiscountPercentage / 100m;
        TotalAmount = grossAmount - DiscountAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new BusinessRuleException("Item ja esta cancelado.");

        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetProduct(Guid productExternalId, string productName)
    {
        if (productExternalId == Guid.Empty)
            throw new BusinessRuleException("O identificador externo do produto e obrigatorio.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new BusinessRuleException("O nome do produto e obrigatorio.");

        ProductExternalId = productExternalId;
        ProductName = productName.Trim();
    }
}
