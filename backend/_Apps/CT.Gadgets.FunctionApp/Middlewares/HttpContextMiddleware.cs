using CT.Gadgets.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace CT.Gadgets.FunctionApp.Middlewares;

public class HttpContextMiddleware : IFunctionsWorkerMiddleware
{
    private readonly HttpRequestContextAccessor _httpContextAccessor;

    public HttpContextMiddleware(HttpRequestContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpRequestData = await context.GetHttpRequestDataAsync();

        _httpContextAccessor.HttpRequestData = httpRequestData;

        await next(context);
    }
}
