namespace Ambev.DeveloperEvaluation.WebApi.Configuration;

public sealed class MetricsOptions
{
    public bool Enabled { get; set; } = true;
    public bool EnablePrometheusEndpoint { get; set; } = true;
    public bool EnableSalesMetrics { get; set; } = true;
    public bool EnableCacheMetrics { get; set; } = true;
}
