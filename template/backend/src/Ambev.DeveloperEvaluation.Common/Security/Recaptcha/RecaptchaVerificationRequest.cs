namespace Ambev.DeveloperEvaluation.Common.Security.Recaptcha;

public sealed class RecaptchaVerificationRequest
{
    public string Token { get; set; } = string.Empty;
    public string ExpectedAction { get; set; } = string.Empty;
    public string? RemoteIp { get; set; }
}
