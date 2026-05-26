namespace Ambev.DeveloperEvaluation.Common.Security.Recaptcha;

public sealed class RecaptchaSimulatedOptions
{
    public bool Enabled { get; set; } = true;
    public string AcceptedTokenPrefix { get; set; } = "simulated";
    public int TokenLifetimeSeconds { get; set; } = 120;
    public double DefaultScore { get; set; } = 0.9;
    public bool ForceFailure { get; set; }
}
