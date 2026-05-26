using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ambev.DeveloperEvaluation.Common.Security.Recaptcha;

public sealed class SimulatedRecaptchaVerifier : IRecaptchaVerifier
{
    private readonly RecaptchaOptions _options;
    private readonly ILogger<SimulatedRecaptchaVerifier> _logger;

    public SimulatedRecaptchaVerifier(IOptions<RecaptchaOptions> options, ILogger<SimulatedRecaptchaVerifier> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<RecaptchaVerificationResult> VerifyAsync(string token, string expectedAction, string? remoteIp, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_options.Enabled)
        {
            _logger.LogInformation("RecaptchaDisabled Provider={Provider} Action={Action}", _options.Provider, expectedAction);
            return Task.FromResult(Success(expectedAction, _options.Simulated.DefaultScore, "RecaptchaDisabled"));
        }

        if (!_options.Simulated.Enabled || !string.Equals(_options.Provider, "Simulated", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(Failure(expectedAction, "SimulatedProviderDisabled", "provider-disabled"));

        if (_options.Simulated.ForceFailure)
        {
            _logger.LogWarning("RecaptchaSimulatedForcedFailure Provider={Provider} Action={Action} RemoteIp={RemoteIp}", _options.Provider, expectedAction, remoteIp);
            return Task.FromResult(Failure(expectedAction, "SimulatedForcedFailure", "forced-failure", 0.1));
        }

        if (string.IsNullOrWhiteSpace(token))
            return Task.FromResult(Failure(expectedAction, "MissingToken", "missing-token"));

        var parts = token.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length != 4)
            return Task.FromResult(Failure(expectedAction, "InvalidTokenFormat", "invalid-format"));

        if (!string.Equals(parts[0], _options.Simulated.AcceptedTokenPrefix, StringComparison.Ordinal))
            return Task.FromResult(Failure(expectedAction, "InvalidTokenPrefix", "invalid-prefix"));

        var action = parts[1];
        if (!string.Equals(action, expectedAction, StringComparison.Ordinal))
            return Task.FromResult(Failure(expectedAction, "InvalidAction", "invalid-action"));

        if (!long.TryParse(parts[2], out var timestampSeconds))
            return Task.FromResult(Failure(expectedAction, "InvalidTimestamp", "invalid-timestamp"));

        var challengeTimestamp = DateTimeOffset.FromUnixTimeSeconds(timestampSeconds);
        var age = DateTimeOffset.UtcNow - challengeTimestamp;
        if (age < TimeSpan.Zero || age > TimeSpan.FromSeconds(_options.Simulated.TokenLifetimeSeconds))
            return Task.FromResult(Failure(expectedAction, "ExpiredToken", "expired-token", _options.Simulated.DefaultScore, challengeTimestamp));

        var score = _options.Simulated.DefaultScore;
        if (score < _options.MinimumScore)
            return Task.FromResult(Failure(expectedAction, "ScoreBelowThreshold", "score-below-threshold", score, challengeTimestamp));

        _logger.LogInformation("RecaptchaVerificationSucceeded Provider={Provider} Action={Action} Score={Score} RemoteIp={RemoteIp}", _options.Provider, expectedAction, score, remoteIp);
        return Task.FromResult(Success(expectedAction, score, string.Empty, challengeTimestamp));
    }

    private static RecaptchaVerificationResult Success(string action, double score, string failureReason, DateTimeOffset? challengeTimestamp = null)
    {
        return new RecaptchaVerificationResult
        {
            Success = true,
            Score = score,
            Action = action,
            ChallengeTimestamp = challengeTimestamp,
            FailureReason = failureReason,
            Provider = "Simulated"
        };
    }

    private RecaptchaVerificationResult Failure(string action, string failureReason, string errorCode, double score = 0, DateTimeOffset? challengeTimestamp = null)
    {
        _logger.LogWarning("RecaptchaVerificationFailed Provider={Provider} Action={Action} Score={Score} FailureReason={FailureReason}", _options.Provider, action, score, failureReason);

        return new RecaptchaVerificationResult
        {
            Success = false,
            Score = score,
            Action = action,
            ChallengeTimestamp = challengeTimestamp,
            FailureReason = failureReason,
            ErrorCodes = [errorCode],
            Provider = "Simulated"
        };
    }
}
