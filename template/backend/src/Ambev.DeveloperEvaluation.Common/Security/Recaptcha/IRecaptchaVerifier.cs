namespace Ambev.DeveloperEvaluation.Common.Security.Recaptcha;

public interface IRecaptchaVerifier
{
    Task<RecaptchaVerificationResult> VerifyAsync(
        string token,
        string expectedAction,
        string? remoteIp,
        CancellationToken cancellationToken);
}
