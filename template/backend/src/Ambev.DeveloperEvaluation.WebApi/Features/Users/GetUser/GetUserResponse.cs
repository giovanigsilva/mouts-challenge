using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

/// <summary>
/// Response de consulta de usuario.
/// </summary>
public class GetUserResponse
{
    /// <summary>
    /// Identificador unico do usuario.
    /// </summary>
    /// <example>8d7e1d6b-0cc1-44fd-aec4-1efc93968fa1</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do usuario.
    /// </summary>
    /// <example>Joao Silva</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuario.
    /// </summary>
    /// <example>joao.silva@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone do usuario.
    /// </summary>
    /// <example>11999999999</example>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Perfil do usuario no sistema.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Status atual do usuario.
    /// </summary>
    public UserStatus Status { get; set; }
}
