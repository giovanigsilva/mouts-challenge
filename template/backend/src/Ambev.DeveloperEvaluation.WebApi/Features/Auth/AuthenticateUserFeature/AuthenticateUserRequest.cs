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
}
