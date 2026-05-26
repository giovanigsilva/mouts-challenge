namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;

/// <summary>
/// Request para remocao de usuario.
/// </summary>
public class DeleteUserRequest
{
    /// <summary>
    /// Identificador unico do usuario que sera removido.
    /// </summary>
    /// <example>8d7e1d6b-0cc1-44fd-aec4-1efc93968fa1</example>
    public Guid Id { get; set; }
}
