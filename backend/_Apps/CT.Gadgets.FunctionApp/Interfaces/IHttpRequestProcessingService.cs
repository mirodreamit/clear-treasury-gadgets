using CT.Application.Abstractions.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CT.Gadgets.FunctionApp.Interfaces;

public interface IHttpRequestProcessingService
{
    Task<HttpResponseData> ProcessHttpRequestAsync( 
        HttpRequestData req,
        Func<Task<IBaseOutput>> f,
        ILogger log);
}
