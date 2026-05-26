using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Swashbuckle.AspNetCore.Annotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth;

/// <summary>
/// Endpoints de autenticacao da API.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Autenticacao")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instancia do controlador de autenticacao.
    /// </summary>
    /// <param name="mediator">Instancia do MediatR usada para executar o caso de uso.</param>
    /// <param name="mapper">Instancia do AutoMapper usada para converter request e response.</param>
    public AuthController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Autentica um usuario e retorna um token JWT.
    /// </summary>
    /// <param name="request">Credenciais do usuario.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao.</param>
    /// <returns>Token JWT e dados basicos do usuario autenticado.</returns>
    [HttpPost]
    [SwaggerOperation(OperationId = "Auth_AuthenticateUser", Summary = "Autentica um usuario.", Description = "Valida email e senha, executa o fluxo de autenticacao existente e retorna um token JWT. Use o token retornado no botao Authorize do Swagger no formato Bearer {token}.")]
    [ProducesResponseType(typeof(ApiResponseWithData<AuthenticateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        var validator = new AuthenticateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<AuthenticateUserCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return new OkObjectResult(new ApiResponseWithData<AuthenticateUserResponse>
        {
            Success = true,
            Message = "User authenticated successfully",
            Data = _mapper.Map<AuthenticateUserResponse>(response)
        });
    }
}
