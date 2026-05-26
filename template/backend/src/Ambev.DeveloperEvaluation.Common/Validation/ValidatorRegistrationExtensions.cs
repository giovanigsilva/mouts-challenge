using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.Common.Validation;

public static class ValidatorRegistrationExtensions
{
    public static IServiceCollection AddValidatorsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        var validatorTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Select(type => new
            {
                ImplementationType = type,
                ValidatorInterfaces = type.GetInterfaces()
                    .Where(current => current.IsGenericType && current.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .ToArray()
            })
            .Where(current => current.ValidatorInterfaces.Length > 0);

        foreach (var validatorType in validatorTypes)
        {
            foreach (var validatorInterface in validatorType.ValidatorInterfaces)
            {
                services.AddTransient(validatorInterface, validatorType.ImplementationType);
            }
        }

        return services;
    }
}
