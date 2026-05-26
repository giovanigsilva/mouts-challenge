using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Common.Behaviors;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddAutoMapper(_ => { }, typeof(ApplicationLayer).Assembly);
        builder.Services.AddValidatorsFromAssemblies(typeof(ApplicationLayer).Assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationLayer).Assembly));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionLoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}
