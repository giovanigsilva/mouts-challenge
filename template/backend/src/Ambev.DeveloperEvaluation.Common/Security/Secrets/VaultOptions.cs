namespace Ambev.DeveloperEvaluation.Common.Security.Secrets;

public sealed class VaultOptions
{
    public bool Enabled { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Mount { get; set; } = "secret";
    public string Path { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 5;
    public bool FailFastInProduction { get; set; } = true;
}
