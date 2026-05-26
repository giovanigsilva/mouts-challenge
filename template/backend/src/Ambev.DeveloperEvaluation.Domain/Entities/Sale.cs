using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    private readonly List<SaleItem> _items = [];
    private readonly List<IDomainEvent> _domainEvents = [];

    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public Guid CustomerExternalId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid BranchExternalId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Sale()
    {
    }

    public Sale(string saleNumber, DateTime saleDate, Guid customerExternalId, string customerName, Guid branchExternalId, string branchName)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        SetHeader(saleNumber, saleDate, customerExternalId, customerName, branchExternalId, branchName);
        AddDomainEvent(new SaleCreatedEvent(Id));
    }

    public void Update(string saleNumber, DateTime saleDate, Guid customerExternalId, string customerName, Guid branchExternalId, string branchName)
    {
        EnsureNotCancelled();
        SetHeader(saleNumber, saleDate, customerExternalId, customerName, branchExternalId, branchName);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleModifiedEvent(Id));
    }

    public SaleItem AddItem(Guid productExternalId, string productName, int quantity, decimal unitPrice)
    {
        EnsureNotCancelled();
        var item = new SaleItem(productExternalId, productName, quantity, unitPrice);
        item.SetSaleId(Id);
        _items.Add(item);
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleModifiedEvent(Id));
        return item;
    }

    public void ReplaceItems(IEnumerable<SaleItemInput> items)
    {
        EnsureNotCancelled();
        _items.Clear();

        foreach (var item in items)
        {
            AddItem(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleModifiedEvent(Id));
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
        RecalculateTotal();
        AddDomainEvent(new SaleCancelledEvent(Id));
    }

    public void CancelItem(Guid itemId)
    {
        EnsureNotCancelled();
        var item = _items.FirstOrDefault(current => current.Id == itemId);

        if (item == null)
            throw new BusinessRuleException("Item da venda nao encontrado.");

        item.Cancel();
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemCancelledEvent(Id, itemId));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void SetHeader(string saleNumber, DateTime saleDate, Guid customerExternalId, string customerName, Guid branchExternalId, string branchName)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new BusinessRuleException("Numero da venda e obrigatorio.");

        if (customerExternalId == Guid.Empty)
            throw new BusinessRuleException("Cliente e obrigatorio.");

        if (string.IsNullOrWhiteSpace(customerName))
            throw new BusinessRuleException("Nome do cliente e obrigatorio.");

        if (branchExternalId == Guid.Empty)
            throw new BusinessRuleException("Filial e obrigatoria.");

        if (string.IsNullOrWhiteSpace(branchName))
            throw new BusinessRuleException("Nome da filial e obrigatorio.");

        SaleNumber = saleNumber.Trim();
        SaleDate = saleDate;
        CustomerExternalId = customerExternalId;
        CustomerName = customerName.Trim();
        BranchExternalId = branchExternalId;
        BranchName = branchName.Trim();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Math.Round(_items.Where(item => !item.IsCancelled).Sum(item => item.TotalAmount), 2);
    }

    private void EnsureNotCancelled()
    {
        if (IsCancelled)
            throw new BusinessRuleException("Venda cancelada nao permite alteracao de itens.");
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
