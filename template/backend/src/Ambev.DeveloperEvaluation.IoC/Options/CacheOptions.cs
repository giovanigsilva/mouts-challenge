namespace Ambev.DeveloperEvaluation.IoC.Options;

public sealed class CacheOptions
{
    public bool Enabled { get; set; }
    public int DefaultExpirationSeconds { get; set; } = 300;
    public int SalesDetailExpirationSeconds { get; set; } = 600;
    public int SalesListExpirationSeconds { get; set; } = 120;
}
