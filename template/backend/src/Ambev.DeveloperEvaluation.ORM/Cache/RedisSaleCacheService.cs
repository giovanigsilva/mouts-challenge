using Ambev.DeveloperEvaluation.Application.Sales.Caching;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.ORM.Cache;

public sealed class RedisSaleCacheService : ISaleCacheService
{
    private const string ListVersionKey = "sales:v1:list:version";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisSaleCacheService> _logger;

    public RedisSaleCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisSaleCacheService> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
    }

    public async Task<SaleResult?> GetSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        try
        {
            var value = await Database.StringGetAsync(GetSaleKey(saleId));
            return value.HasValue ? JsonSerializer.Deserialize<SaleResult>(value!, JsonOptions) : null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao ler venda do Redis. SaleId: {SaleId}", saleId);
            return null;
        }
    }

    public async Task SetSaleAsync(SaleResult sale, CancellationToken cancellationToken)
    {
        try
        {
            await Database.StringSetAsync(GetSaleKey(sale.Id), JsonSerializer.Serialize(sale, JsonOptions), TimeSpan.FromMinutes(15));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao gravar venda no Redis. SaleId: {SaleId}", sale.Id);
        }
    }

    public async Task<PaginatedSalesResult?> GetSalesAsync(ListSalesCommand query, CancellationToken cancellationToken)
    {
        try
        {
            var version = await GetListVersionAsync();
            var value = await Database.StringGetAsync(GetSalesKey(query, version));
            return value.HasValue ? JsonSerializer.Deserialize<PaginatedSalesResult>(value!, JsonOptions) : null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao ler lista de vendas do Redis.");
            return null;
        }
    }

    public async Task SetSalesAsync(ListSalesCommand query, PaginatedSalesResult result, CancellationToken cancellationToken)
    {
        try
        {
            var version = await GetListVersionAsync();
            await Database.StringSetAsync(GetSalesKey(query, version), JsonSerializer.Serialize(result, JsonOptions), TimeSpan.FromMinutes(5));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao gravar lista de vendas no Redis.");
        }
    }

    public async Task InvalidateSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        try
        {
            await Database.KeyDeleteAsync(GetSaleKey(saleId));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao invalidar venda no Redis. SaleId: {SaleId}", saleId);
        }
    }

    public async Task InvalidateSalesListAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Database.StringIncrementAsync(ListVersionKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao invalidar lista de vendas no Redis.");
        }
    }

    private IDatabase Database => _connectionMultiplexer.GetDatabase();

    private static string GetSaleKey(Guid saleId)
    {
        return $"sales:v1:details:{saleId}";
    }

    private async Task<long> GetListVersionAsync()
    {
        var value = await Database.StringGetAsync(ListVersionKey);
        if (value.HasValue && long.TryParse(value!, out var version))
            return version;

        await Database.StringSetAsync(ListVersionKey, 1);
        return 1;
    }

    private static string GetSalesKey(ListSalesCommand query, long version)
    {
        return $"sales:v1:list:v{version}:page:{query.Page}:size:{query.PageSize}:saleNumber:{query.SaleNumber}:customer:{query.CustomerId}:branch:{query.BranchId}:cancelled:{query.IsCancelled}:from:{query.FromDate:O}:to:{query.ToDate:O}";
    }
}
