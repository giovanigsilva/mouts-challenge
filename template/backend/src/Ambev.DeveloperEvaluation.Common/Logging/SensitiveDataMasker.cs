namespace Ambev.DeveloperEvaluation.Common.Logging;

using System.Text.RegularExpressions;

public static class SensitiveDataMasker
{
    public const string Mask = "***MASKED***";

    private static readonly string[] SensitiveTerms =
    [
        "password",
        "senha",
        "token",
        "accesstoken",
        "refreshtoken",
        "jwt",
        "authorization",
        "secret",
        "clientsecret",
        "connectionstring",
        "defaultconnection",
        "apikey",
        "api_key",
        "key",
        "credential"
    ];

    public static string? MaskIfSensitive(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return value;

        return IsSensitiveKey(key) ? Mask : value;
    }

    public static IReadOnlyDictionary<string, string?> MaskDictionary(IReadOnlyDictionary<string, string?> values)
    {
        return values.ToDictionary(item => item.Key, item => MaskIfSensitive(item.Key, item.Value));
    }

    public static bool IsSensitiveKey(string key)
    {
        var normalized = key.Replace("_", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        return SensitiveTerms.Any(term => normalized.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    public static string MaskSensitiveText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var masked = text;
        masked = Regex.Replace(masked, "(authorization\\s*[:=]\\s*)[^\\s;]+", $"$1{Mask}", RegexOptions.IgnoreCase);
        masked = Regex.Replace(masked, "(bearer\\s+)[^\\s;]+", $"$1{Mask}", RegexOptions.IgnoreCase);
        masked = Regex.Replace(masked, "((password|senha|token|secret|apikey|api_key|credential|connectionstring)\\s*[:=]\\s*)[^;\\s]+", $"$1{Mask}", RegexOptions.IgnoreCase);
        return masked;
    }
}
