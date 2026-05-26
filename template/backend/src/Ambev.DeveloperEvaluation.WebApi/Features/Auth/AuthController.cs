using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security.Recaptcha;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth;

/// <summary>
/// Endpoints de autenticacao da API.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Autenticacao")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IRecaptchaVerifier _recaptchaVerifier;
    private readonly RecaptchaOptions _recaptchaOptions;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Inicializa uma nova instancia do controlador de autenticacao.
    /// </summary>
    /// <param name="mediator">Instancia do MediatR usada para executar o caso de uso.</param>
    /// <param name="mapper">Instancia do AutoMapper usada para converter request e response.</param>
    /// <param name="recaptchaVerifier">Servico de verificacao anti-robo em modo simulado.</param>
    /// <param name="recaptchaOptions">Configuracoes de reCAPTCHA usadas pelo endpoint.</param>
    /// <param name="logger">Logger estruturado do controlador.</param>
    public AuthController(IMediator mediator, IMapper mapper, IRecaptchaVerifier recaptchaVerifier, IOptions<RecaptchaOptions> recaptchaOptions, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _recaptchaVerifier = recaptchaVerifier;
        _recaptchaOptions = recaptchaOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Autentica um usuario e retorna um token JWT.
    /// </summary>
    /// <param name="request">Credenciais do usuario.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao.</param>
    /// <returns>Token JWT e dados basicos do usuario autenticado.</returns>
    [HttpPost]
    [SwaggerOperation(OperationId = "Auth_AuthenticateUser", Summary = "Autentica um usuario.", Description = "Valida a protecao anti-robo simulada quando Recaptcha:Enabled=true, valida email e senha, executa o fluxo de autenticacao existente e retorna um token JWT. No modo simulado, envie recaptchaToken no formato simulated:login:{timestamp}:{nonce}. Use o token JWT retornado no botao Authorize do Swagger no formato Bearer {token}.")]
    [ProducesResponseType(typeof(ApiResponseWithData<AuthenticateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        var validator = new AuthenticateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var recaptcha = await _recaptchaVerifier.VerifyAsync(request.RecaptchaToken ?? string.Empty, _recaptchaOptions.Actions.Login, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
        if (!recaptcha.Success)
        {
            _logger.LogWarning("RecaptchaLoginRejected Provider={Provider} Action={Action} Score={Score} FailureReason={FailureReason}", recaptcha.Provider, recaptcha.Action, recaptcha.Score, recaptcha.FailureReason);
            return BadRequest("Nao foi possivel validar a protecao anti-robo. Tente novamente.");
        }

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
