namespace Ambev.DeveloperEvaluation.Common.Security.Secrets;

public interface ISecretProvider
{
    Task<string?> GetSecretAsync(string key, CancellationToken cancellationToken);
    Task<IDictionary<string, string>> GetSecretsAsync(CancellationToken cancellationToken);
}
