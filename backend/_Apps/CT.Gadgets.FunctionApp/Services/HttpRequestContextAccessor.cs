using Microsoft.Azure.Functions.Worker.Http;

namespace CT.Gadgets.FunctionApp.Services;

public class HttpRequestContextAccessor
{
    public HttpRequestData? HttpRequestData { get; set; }
}
