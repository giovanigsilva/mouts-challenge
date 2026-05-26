using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser
{
    public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthenticateUserHandler> _logger;

        public AuthenticateUserHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<AuthenticateUserHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<AuthenticateUserResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} ActorUserEmail={ActorUserEmail} OccurredAtUtc={OccurredAtUtc}", "LoginAttempt", "AuthenticateUser", "Started", "User", request.Email, DateTime.UtcNow);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} TargetEntityType={TargetEntityType} ActorUserEmail={ActorUserEmail} OccurredAtUtc={OccurredAtUtc}", "LoginFailed", "AuthenticateUser", "Denied", "User", request.Email, DateTime.UtcNow);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var activeUserSpec = new ActiveUserSpecification();
            if (!activeUserSpec.IsSatisfiedBy(user))
            {
                _logger.LogWarning("AuditEvent={AuditEventName} Action={Action} Result={Result} ActorUserId={ActorUserId} ActorUserName={ActorUserName} ActorUserEmail={ActorUserEmail} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "LoginFailed", "AuthenticateUser", "InactiveUser", user.Id, user.Username, user.Email, "User", user.Id, DateTime.UtcNow);
                throw new UnauthorizedAccessException("User is not active");
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} ActorUserId={ActorUserId} ActorUserName={ActorUserName} ActorUserEmail={ActorUserEmail} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "LoginSuccess", "AuthenticateUser", "Success", user.Id, user.Username, user.Email, "User", user.Id, DateTime.UtcNow);
            _logger.LogInformation("AuditEvent={AuditEventName} Action={Action} Result={Result} ActorUserId={ActorUserId} TargetEntityType={TargetEntityType} TargetEntityId={TargetEntityId} OccurredAtUtc={OccurredAtUtc}", "TokenIssued", "IssueJwtToken", "Success", user.Id, "User", user.Id, DateTime.UtcNow);

            return new AuthenticateUserResult
            {
                Token = token,
                Email = user.Email,
                Name = user.Username,
                Role = user.Role.ToString()
            };
        }
    }
}
