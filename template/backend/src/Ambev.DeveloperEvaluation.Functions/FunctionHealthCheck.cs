using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Ambev.DeveloperEvaluation.Functions;

public sealed class FunctionHealthCheck
{
    [Function(nameof(FunctionHealthCheck))]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "health")] HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.WriteAsJsonAsync(new
        {
            status = "Healthy",
            service = "DeveloperStore Sales Functions"
        });
        return response;
    }
}
