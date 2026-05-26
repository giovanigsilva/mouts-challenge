using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Envelope padrao usado pelas respostas da API.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indica se a operacao foi executada com sucesso.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem resumida sobre o resultado da operacao.
    /// </summary>
    /// <example>Operacao realizada com sucesso.</example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Lista de erros de validacao quando a requisicao possuir campos invalidos.
    /// </summary>
    public IEnumerable<ValidationErrorDetail> Errors { get; set; } = [];
}
