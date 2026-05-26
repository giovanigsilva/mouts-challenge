namespace Ambev.DeveloperEvaluation.Common.Security.Recaptcha;

public sealed class RecaptchaVerificationResult
{
    public bool Success { get; set; }
    public double Score { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Hostname { get; set; } = "local";
    public DateTimeOffset? ChallengeTimestamp { get; set; }
    public IReadOnlyCollection<string> ErrorCodes { get; set; } = [];
    public string FailureReason { get; set; } = string.Empty;
    public string Provider { get; set; } = "Simulated";
}
