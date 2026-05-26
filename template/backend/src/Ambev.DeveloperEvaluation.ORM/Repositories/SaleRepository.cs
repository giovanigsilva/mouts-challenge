using Ambev.DeveloperEvaluation.Application.IntegrationEvents.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;
    private readonly ILogger<SaleRepository> _logger;

    public SaleRepository(DefaultContext context, ILogger<SaleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await AddOutboxMessagesAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        sale.ClearDomainEvents();
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Sale>> ListAsync(string? saleNumber, Guid? customerId, Guid? branchId, bool? isCancelled, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(_context.Sales.Include(sale => sale.Items).AsQueryable(), saleNumber, customerId, branchId, isCancelled, fromDate, toDate)
            .OrderByDescending(sale => sale.SaleDate)
            .ThenByDescending(sale => sale.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? saleNumber, Guid? customerId, Guid? branchId, bool? isCancelled, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(_context.Sales.AsQueryable(), saleNumber, customerId, branchId, isCancelled, fromDate, toDate)
            .CountAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await AddOutboxMessagesAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        sale.ClearDomainEvents();
    }

    public async Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, string? saleNumber, Guid? customerId, Guid? branchId, bool? isCancelled, DateTime? fromDate, DateTime? toDate)
    {
        if (!string.IsNullOrWhiteSpace(saleNumber))
            query = query.Where(sale => sale.SaleNumber.Contains(saleNumber));

        if (customerId.HasValue)
            query = query.Where(sale => sale.CustomerExternalId == customerId.Value);

        if (branchId.HasValue)
            query = query.Where(sale => sale.BranchExternalId == branchId.Value);

        if (isCancelled.HasValue)
            query = query.Where(sale => sale.IsCancelled == isCancelled.Value);

        if (fromDate.HasValue)
            query = query.Where(sale => sale.SaleDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(sale => sale.SaleDate <= toDate.Value);

        return query;
    }

    private async Task AddOutboxMessagesAsync(Sale sale, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in sale.DomainEvents)
        {
            var outboxMessage = CreateOutboxMessage(domainEvent);
            await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            _logger.LogInformation("Evento de venda registrado na Outbox. EventId: {EventId}, EventType: {EventType}, AggregateId: {AggregateId}", outboxMessage.EventId, outboxMessage.EventType, outboxMessage.AggregateId);
        }
    }

    private static OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventId = domainEvent.EventId,
            EventType = DomainEventToIntegrationEventMapper.GetEventType(domainEvent),
            AggregateId = domainEvent.AggregateId,
            AggregateType = "Sale",
            Payload = JsonSerializer.Serialize(DomainEventToIntegrationEventMapper.Map(domainEvent)),
            Headers = JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                source = "DeveloperStore.Sales.Api"
            }),
            Status = OutboxMessageStatus.Pending,
            RetryCount = 0,
            MaxRetries = 5,
            CreatedAt = DateTime.UtcNow
        };
    }
}
