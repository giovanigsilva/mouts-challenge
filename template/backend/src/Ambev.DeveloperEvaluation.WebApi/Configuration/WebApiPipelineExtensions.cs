using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.WebApi.HealthChecks;
using Ambev.DeveloperEvaluation.WebApi.Middleware;

namespace Ambev.DeveloperEvaluation.WebApi.Configuration;

public static class WebApiPipelineExtensions
{
    public static WebApplication UseWebApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        if (app.Configuration.GetValue<bool>("Swagger:Enabled"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Configuration.GetValue<bool>("Security:EnableHsts") && !app.Environment.IsDevelopment())
            app.UseHsts();

        if (app.Configuration.GetValue<bool>("Security:EnableHttpsRedirection"))
            app.UseHttpsRedirection();

        if (app.Configuration.GetValue<bool>("Security:EnableSecurityHeaders"))
            app.UseMiddleware<SecurityHeadersMiddleware>();

        app.UseCors("DefaultCors");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseBasicHealthChecks();
        app.MapLoggingHealthCheck();
        app.MapControllers();

        return app;
    }
}
