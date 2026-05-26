using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// Request para criacao de usuario.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Nome do usuario.
    /// </summary>
    /// <example>Joao Silva</example>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuario.
    /// </summary>
    /// <example>Senha@123456</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Telefone do usuario.
    /// </summary>
    /// <example>11999999999</example>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuario.
    /// </summary>
    /// <example>joao.silva@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Status inicial da conta do usuario.
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Perfil atribuido ao usuario.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Token de protecao anti-robo. No modo simulado, usa o formato simulated:create_user:{timestamp}:{nonce}.
    /// </summary>
    /// <example>simulated:create_user:1779800000:0f5c4c8e</example>
    public string? RecaptchaToken { get; set; }
}
