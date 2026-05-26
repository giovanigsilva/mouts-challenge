using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Validation;

public sealed class ValidatorRegistrationExtensionsTests
{
    [Fact(DisplayName = "Given application assembly When registering validators Then command validators are available")]
    public void AddValidatorsFromAssemblies_ApplicationAssembly_RegistersCommandValidators()
    {
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblies(typeof(ApplicationLayer).Assembly);

        using var provider = services.BuildServiceProvider();
        var validator = provider.GetService<IValidator<CreateSaleCommand>>();

        validator.Should().BeOfType<CreateSaleValidator>();
    }

    [Fact(DisplayName = "Given webapi assembly When registering validators Then request validators are available")]
    public void AddValidatorsFromAssemblies_WebApiAssembly_RegistersRequestValidators()
    {
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblies(typeof(Program).Assembly);

        using var provider = services.BuildServiceProvider();
        var validator = provider.GetService<IValidator<CreateSaleRequest>>();

        validator.Should().BeOfType<CreateSaleRequestValidator>();
    }
}
