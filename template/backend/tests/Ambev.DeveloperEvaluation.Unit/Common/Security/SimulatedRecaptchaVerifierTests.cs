using Ambev.DeveloperEvaluation.Common.Security.Recaptcha;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Security;

public sealed class SimulatedRecaptchaVerifierTests
{
    [Fact(DisplayName = "Given valid simulated token When verifying Then succeeds")]
    public async Task VerifyAsync_ValidToken_Succeeds()
    {
        var verifier = CreateVerifier();
        var token = CreateToken("login");

        var result = await verifier.VerifyAsync(token, "login", "127.0.0.1", CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Score.Should().Be(0.9);
        result.Action.Should().Be("login");
    }

    [Fact(DisplayName = "Given empty token and enabled recaptcha When verifying Then fails")]
    public async Task VerifyAsync_EmptyToken_Fails()
    {
        var verifier = CreateVerifier();

        var result = await verifier.VerifyAsync(string.Empty, "login", null, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().Be("MissingToken");
    }

    [Fact(DisplayName = "Given invalid prefix When verifying Then fails")]
    public async Task VerifyAsync_InvalidPrefix_Fails()
    {
        var verifier = CreateVerifier();

        var result = await verifier.VerifyAsync(CreateToken("login", "invalid"), "login", null, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().Be("InvalidTokenPrefix");
    }

    [Fact(DisplayName = "Given different action When verifying Then fails")]
    public async Task VerifyAsync_DifferentAction_Fails()
    {
        var verifier = CreateVerifier();

        var result = await verifier.VerifyAsync(CreateToken("create_user"), "login", null, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().Be("InvalidAction");
    }

    [Fact(DisplayName = "Given expired token When verifying Then fails")]
    public async Task VerifyAsync_ExpiredToken_Fails()
    {
        var verifier = CreateVerifier();
        var timestamp = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds();

        var result = await verifier.VerifyAsync($"simulated:login:{timestamp}:{Guid.NewGuid()}", "login", null, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().Be("ExpiredToken");
    }

    [Fact(DisplayName = "Given score below threshold When verifying Then fails")]
    public async Task VerifyAsync_ScoreBelowThreshold_Fails()
    {
        var verifier = CreateVerifier(options =>
        {
            options.MinimumScore = 0.95;
            options.Simulated.DefaultScore = 0.9;
        });

        var result = await verifier.VerifyAsync(CreateToken("login"), "login", null, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().Be("ScoreBelowThreshold");
    }

    [Fact(DisplayName = "Given forced failure When verifying Then fails")]
    public async Task VerifyAsync_ForcedFailure_Fails()
    {
        var verifier = CreateVerifier(options => options.Simulated.ForceFailure = true);

        var result = await verifier.VerifyAsync(CreateToken("login"), "login", null, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().Be("SimulatedForcedFailure");
    }

    [Fact(DisplayName = "Given recaptcha disabled When verifying Then succeeds")]
    public async Task VerifyAsync_Disabled_Succeeds()
    {
        var verifier = CreateVerifier(options => options.Enabled = false);

        var result = await verifier.VerifyAsync(string.Empty, "login", null, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.FailureReason.Should().Be("RecaptchaDisabled");
    }

    private static SimulatedRecaptchaVerifier CreateVerifier(Action<RecaptchaOptions>? configure = null)
    {
        var options = new RecaptchaOptions
        {
            Enabled = true,
            Provider = "Simulated",
            MinimumScore = 0.5,
            Simulated = new RecaptchaSimulatedOptions
            {
                Enabled = true,
                AcceptedTokenPrefix = "simulated",
                TokenLifetimeSeconds = 120,
                DefaultScore = 0.9
            }
        };
        configure?.Invoke(options);

        return new SimulatedRecaptchaVerifier(Options.Create(options), NullLogger<SimulatedRecaptchaVerifier>.Instance);
    }

    private static string CreateToken(string action, string prefix = "simulated")
    {
        return $"{prefix}:{action}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}:{Guid.NewGuid()}";
    }
}
