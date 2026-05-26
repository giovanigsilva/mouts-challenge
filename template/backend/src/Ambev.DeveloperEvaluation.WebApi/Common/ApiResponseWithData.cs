namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Envelope padrao usado pelas respostas da API que retornam dados.
/// </summary>
/// <typeparam name="T">Tipo do dado retornado pela operacao.</typeparam>
public class ApiResponseWithData<T> : ApiResponse
{
    /// <summary>
    /// Dados retornados pela operacao.
    /// </summary>
    public T? Data { get; set; }
}
