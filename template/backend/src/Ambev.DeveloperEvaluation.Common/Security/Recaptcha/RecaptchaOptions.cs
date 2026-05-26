namespace Ambev.DeveloperEvaluation.Common.Security.Recaptcha;

public sealed class RecaptchaOptions
{
    public bool Enabled { get; set; }
    public string Provider { get; set; } = "Simulated";
    public double MinimumScore { get; set; } = 0.5;
    public RecaptchaActionOptions Actions { get; set; } = new();
    public RecaptchaSimulatedOptions Simulated { get; set; } = new();
}
