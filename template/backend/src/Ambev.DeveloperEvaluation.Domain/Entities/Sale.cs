using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    private readonly List<SaleItem> _items = [];
    private readonly List<object> _domainEvents = [];

    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public Guid CustomerExternalId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid BranchExternalId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    private Sale()
    {
    }

    public Sale(string saleNumber, DateTime saleDate, Guid customerExternalId, string customerName, Guid branchExternalId, string branchName, IEnumerable<SaleItem> items)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        SetHeader(saleNumber, saleDate, customerExternalId, customerName, branchExternalId, branchName);
        ReplaceItems(items);
        AddDomainEvent(new SaleCreatedEvent(this));
    }

    public void Update(string saleNumber, DateTime saleDate, Guid customerExternalId, string customerName, Guid branchExternalId, string branchName, IEnumerable<SaleItem> items)
    {
        EnsureCanChangeItems();
        SetHeader(saleNumber, saleDate, customerExternalId, customerName, branchExternalId, branchName);
        ReplaceItems(items);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleModifiedEvent(this));
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new BusinessRuleException("Venda ja esta cancelada.");

        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleCancelledEvent(this));
    }

    public void CancelItem(Guid itemId)
    {
        EnsureCanChangeItems();

        var item = _items.FirstOrDefault(current => current.Id == itemId);
        if (item is null)
            throw new BusinessRuleException("Item da venda nao encontrado.");

        item.Cancel();
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemCancelledEvent(this, item));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void ReplaceItems(IEnumerable<SaleItem> items)
    {
        var saleItems = items?.ToList() ?? [];
        if (saleItems.Count == 0)
            throw new BusinessRuleException("A venda deve possuir ao menos um item.");

        var hasDuplicatedProduct = saleItems
            .GroupBy(item => item.ProductExternalId)
            .Any(group => group.Count() > 1);

        if (hasDuplicatedProduct)
            throw new BusinessRuleException("Nao e permitido repetir o mesmo produto na venda.");

        _items.Clear();
        _items.AddRange(saleItems);
        RecalculateTotal();
    }

    private void SetHeader(string saleNumber, DateTime saleDate, Guid customerExternalId, string customerName, Guid branchExternalId, string branchName)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new BusinessRuleException("O numero da venda e obrigatorio.");

        if (customerExternalId == Guid.Empty)
            throw new BusinessRuleException("O identificador externo do cliente e obrigatorio.");

        if (string.IsNullOrWhiteSpace(customerName))
            throw new BusinessRuleException("O nome do cliente e obrigatorio.");

        if (branchExternalId == Guid.Empty)
            throw new BusinessRuleException("O identificador externo da filial e obrigatorio.");

        if (string.IsNullOrWhiteSpace(branchName))
            throw new BusinessRuleException("O nome da filial e obrigatorio.");

        SaleNumber = saleNumber.Trim();
        SaleDate = saleDate;
        CustomerExternalId = customerExternalId;
        CustomerName = customerName.Trim();
        BranchExternalId = branchExternalId;
        BranchName = branchName.Trim();
    }

    private void EnsureCanChangeItems()
    {
        if (IsCancelled)
            throw new BusinessRuleException("Venda cancelada nao permite alteracao de itens.");
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items
            .Where(item => !item.IsCancelled)
            .Sum(item => item.TotalAmount);
    }

    private void AddDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
