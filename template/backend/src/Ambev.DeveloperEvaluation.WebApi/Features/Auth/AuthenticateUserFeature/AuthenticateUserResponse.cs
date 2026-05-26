namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

/// <summary>
/// Response retornado apos autenticacao bem-sucedida.
/// </summary>
public sealed class AuthenticateUserResponse
{
    /// <summary>
    /// Token JWT que deve ser usado no header Authorization.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuario autenticado.
    /// </summary>
    /// <example>admin@developerstore.com</example>
    public string Email { get; set; } = string.Empty;   

    /// <summary>
    /// Nome do usuario autenticado.
    /// </summary>
    /// <example>Administrador DeveloperStore</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Perfil do usuario autenticado.
    /// </summary>
    /// <example>Admin</example>
    public string Role { get; set; } = string.Empty;
}
