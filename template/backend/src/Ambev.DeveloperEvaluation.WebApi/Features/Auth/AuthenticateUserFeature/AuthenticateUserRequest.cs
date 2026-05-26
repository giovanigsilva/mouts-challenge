namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

/// <summary>
/// Request de autenticacao de usuario.
/// </summary>
public class AuthenticateUserRequest
{
    /// <summary>
    /// Email usado para autenticar o usuario.
    /// </summary>
    /// <example>admin@developerstore.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuario.
    /// </summary>
    /// <example>Senha@123456</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Token de protecao anti-robo. No modo simulado, usa o formato simulated:login:{timestamp}:{nonce}.
    /// </summary>
    /// <example>simulated:login:1779800000:0f5c4c8e</example>
    public string? RecaptchaToken { get; set; }
}
