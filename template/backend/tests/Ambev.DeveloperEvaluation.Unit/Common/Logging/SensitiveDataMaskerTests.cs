using Ambev.DeveloperEvaluation.Common.Logging;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Logging;

public sealed class SensitiveDataMaskerTests
{
    [Theory(DisplayName = "Given sensitive key When masking Then returns masked value")]
    [InlineData("Authorization")]
    [InlineData("password")]
    [InlineData("token")]
    [InlineData("secret")]
    [InlineData("connectionString")]
    [InlineData("apiKey")]
    public void MaskIfSensitive_SensitiveKey_ReturnsMaskedValue(string key)
    {
        var result = SensitiveDataMasker.MaskIfSensitive(key, "sensitive-value");

        result.Should().Be(SensitiveDataMasker.Mask);
    }

    [Fact(DisplayName = "Given safe key When masking Then keeps value")]
    public void MaskIfSensitive_SafeKey_KeepsValue()
    {
        var result = SensitiveDataMasker.MaskIfSensitive("saleNumber", "SALE-2026-000001");

        result.Should().Be("SALE-2026-000001");
    }

    [Fact(DisplayName = "Given text with secrets When masking Then hides sensitive values")]
    public void MaskSensitiveText_TextWithSecrets_HidesSensitiveValues()
    {
        var text = "Authorization: Bearer abc; password=123; connectionString=Host=localhost";

        var result = SensitiveDataMasker.MaskSensitiveText(text);

        result.Should().Contain(SensitiveDataMasker.Mask);
        result.Should().NotContain("abc");
        result.Should().NotContain("123");
        result.Should().NotContain("Host=localhost");
    }
}
