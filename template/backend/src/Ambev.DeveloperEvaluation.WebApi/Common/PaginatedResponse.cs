namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Envelope padrao usado pelas respostas paginadas da API.
/// </summary>
/// <typeparam name="T">Tipo dos itens retornados na pagina.</typeparam>
public class PaginatedResponse<T> : ApiResponseWithData<IEnumerable<T>>
{
    /// <summary>
    /// Numero da pagina atual.
    /// </summary>
    /// <example>1</example>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Quantidade total de paginas disponiveis.
    /// </summary>
    /// <example>5</example>
    public int TotalPages { get; set; }

    /// <summary>
    /// Quantidade total de registros encontrados.
    /// </summary>
    /// <example>100</example>
    public int TotalCount { get; set; }
}
