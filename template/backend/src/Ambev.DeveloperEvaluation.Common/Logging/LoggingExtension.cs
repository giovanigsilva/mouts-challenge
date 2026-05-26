using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Debugging;
using Serilog.Formatting.Compact;

namespace Ambev.DeveloperEvaluation.Common.Logging;



/// <summary> Add default Logging configuration to project. This configuration supports Serilog logs with DataDog compatible output.</summary>
public static class LoggingExtension
{
    /// <summary>
    /// The destructuring options builder configured with default destructurers and a custom DbUpdateExceptionDestructurer.
    /// </summary>
    static readonly DestructuringOptionsBuilder _destructuringOptionsBuilder = new DestructuringOptionsBuilder()
        .WithDefaultDestructurers()
        .WithDestructurers([new DbUpdateExceptionDestructurer()]);

    /// <summary>
    /// A filter predicate to exclude log events with specific criteria.
    /// </summary>
    static readonly Func<LogEvent, bool> _filterPredicate = exclusionPredicate =>
    {

        if (exclusionPredicate.Level != LogEventLevel.Information) return true;

        exclusionPredicate.Properties.TryGetValue("StatusCode", out var statusCode);
        exclusionPredicate.Properties.TryGetValue("Path", out var path);

        var excludeByStatusCode = statusCode == null || statusCode.ToString().Equals("200");
        var excludeByPath = path?.ToString().Contains("/health") ?? false;

        return excludeByStatusCode && excludeByPath;
    };

    /// <summary>
    /// This method configures the logging with commonly used features for DataDog integration.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> to add services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further configure the API services.</returns>
    /// <remarks>
    /// <para>Logging output is JSON on console and optionally sent to Seq.</para>
    /// </remarks> 
    public static WebApplicationBuilder AddDefaultLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration().CreateLogger();
        if (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Observability:EnableSerilogSelfLog"))
            SelfLog.Enable(message => Console.Error.WriteLine(message));

        builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var configuration = hostingContext.Configuration;
            var seqEnabled = configuration.GetValue<bool>("Seq:Enabled") && configuration.GetValue<bool>("Observability:EnableSeqFallback");
            var seqUrl = configuration["Seq:Url"];
            var serviceName = configuration["Observability:ServiceName"] ?? configuration["Datadog:Service"] ?? builder.Environment.ApplicationName;
            var version = configuration["Application:Version"] ?? configuration["Datadog:Version"] ?? "1.0.0";

            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .Enrich.WithProperty("Application", serviceName)
                .Enrich.WithProperty("Version", version)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails(_destructuringOptionsBuilder)
                .Filter.ByExcluding(_filterPredicate)
                .WriteTo.Async(writeTo => writeTo.Console(new RenderedCompactJsonFormatter()));

            if (seqEnabled && !string.IsNullOrWhiteSpace(seqUrl))
            {
                var apiKey = configuration["Seq:ApiKey"];
                loggerConfiguration.WriteTo.Async(writeTo => writeTo.Seq(seqUrl, apiKey: string.IsNullOrWhiteSpace(apiKey) ? null : apiKey));
            }
        });

        builder.Services.AddLogging();

        return builder;
    }

    /// <summary>Adds middleware for Swagger documetation generation.</summary>
    /// <param name="app">The <see cref="WebApplication"/> instance this method extends.</param>
    /// <returns>The <see cref="WebApplication"/> for Swagger documentation.</returns>
    public static WebApplication UseDefaultLogging(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Logger>>();

        var configuration = app.Configuration;
        logger.LogInformation("Logging enabled ApplicationName={ApplicationName} Version={Version} Environment={Environment} SwaggerEnabled={SwaggerEnabled} SeqEnabled={SeqEnabled} DatadogEnabled={DatadogEnabled} DetailedErrorsEnabled={DetailedErrorsEnabled}",
            configuration["Observability:ServiceName"] ?? app.Environment.ApplicationName,
            configuration["Application:Version"] ?? "1.0.0",
            app.Environment.EnvironmentName,
            configuration.GetValue<bool>("Swagger:Enabled"),
            configuration.GetValue<bool>("Seq:Enabled") && configuration.GetValue<bool>("Observability:EnableSeqFallback"),
            configuration.GetValue<bool>("Datadog:Enabled") && configuration.GetValue<bool>("Observability:EnableDatadog"),
            configuration.GetValue<bool>("Features:EnableDetailedErrors"));
        return app;

    }
}
