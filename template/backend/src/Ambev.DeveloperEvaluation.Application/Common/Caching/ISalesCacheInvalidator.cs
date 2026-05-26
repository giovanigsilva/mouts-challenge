namespace Ambev.DeveloperEvaluation.Application.Common.Caching;

public interface ISalesCacheInvalidator
{
    Task InvalidateAsync(Guid saleId, string operationName, CancellationToken cancellationToken);
}
