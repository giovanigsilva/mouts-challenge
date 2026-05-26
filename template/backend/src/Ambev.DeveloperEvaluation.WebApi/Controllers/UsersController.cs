using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Common.Security.Recaptcha;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace Ambev.DeveloperEvaluation.WebApi.Controllers;

/// <summary>
/// Endpoints de gerenciamento de usuarios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Usuarios")]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IRecaptchaVerifier _recaptchaVerifier;
    private readonly RecaptchaOptions _recaptchaOptions;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Inicializa uma nova instancia do controlador de usuarios.
    /// </summary>
    /// <param name="mediator">Instancia do MediatR usada para executar os casos de uso.</param>
    /// <param name="mapper">Instancia do AutoMapper usada para converter requests e responses.</param>
    /// <param name="recaptchaVerifier">Servico de verificacao anti-robo em modo simulado.</param>
    /// <param name="recaptchaOptions">Configuracoes de reCAPTCHA usadas pelo endpoint.</param>
    /// <param name="logger">Logger estruturado do controlador.</param>
    public UsersController(IMediator mediator, IMapper mapper, IRecaptchaVerifier recaptchaVerifier, IOptions<RecaptchaOptions> recaptchaOptions, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _recaptchaVerifier = recaptchaVerifier;
        _recaptchaOptions = recaptchaOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo usuario.
    /// </summary>
    /// <param name="request">Dados para criacao do usuario.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao.</param>
    /// <returns>Dados do usuario criado.</returns>
    [HttpPost]
    [SwaggerOperation(OperationId = "Users_CreateUser", Summary = "Cria um novo usuario.", Description = "Valida a protecao anti-robo simulada quando Recaptcha:Enabled=true e cria um usuario com nome, email, telefone, senha, status e perfil. No modo simulado, envie recaptchaToken no formato simulated:create_user:{timestamp}:{nonce}. A senha e recebida apenas no request e nao e retornada na resposta.")]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var recaptcha = await _recaptchaVerifier.VerifyAsync(request.RecaptchaToken ?? string.Empty, _recaptchaOptions.Actions.CreateUser, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
        if (!recaptcha.Success)
        {
            _logger.LogWarning("RecaptchaCreateUserRejected Provider={Provider} Action={Action} Score={Score} FailureReason={FailureReason}", recaptcha.Provider, recaptcha.Action, recaptcha.Score, recaptcha.FailureReason);
            return BadRequest("Nao foi possivel validar a protecao anti-robo. Tente novamente.");
        }

        var command = _mapper.Map<CreateUserCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateUserResponse>
        {
            Success = true,
            Message = "User created successfully",
            Data = _mapper.Map<CreateUserResponse>(response)
        });
    }

    /// <summary>
    /// Consulta um usuario por identificador.
    /// </summary>
    /// <param name="id">Identificador unico do usuario.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao.</param>
    /// <returns>Dados do usuario encontrado.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(OperationId = "Users_GetUserById", Summary = "Consulta um usuario por identificador.", Description = "Retorna os dados cadastrais de um usuario pelo identificador informado.")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetUserRequest { Id = id };
        var validator = new GetUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetUserCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetUserResponse>
        {
            Success = true,
            Message = "User retrieved successfully",
            Data = _mapper.Map<GetUserResponse>(response)
        });
    }

    /// <summary>
    /// Remove um usuario por identificador.
    /// </summary>
    /// <param name="id">Identificador unico do usuario que sera removido.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao.</param>
    /// <returns>Resposta de sucesso quando o usuario for removido.</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(OperationId = "Users_DeleteUser", Summary = "Remove um usuario.", Description = "Remove o usuario identificado pelo parametro id, seguindo o comportamento ja implementado no caso de uso de usuarios.")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteUserRequest { Id = id };
        var validator = new DeleteUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteUserCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "User deleted successfully"
        });
    }
}
