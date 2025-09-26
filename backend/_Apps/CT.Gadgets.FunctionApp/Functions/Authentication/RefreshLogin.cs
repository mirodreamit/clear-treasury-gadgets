using CT.Application.Abstractions.Models;
using CT.Application.FeaturesIS.Login.Queries;
using CT.Gadgets.FunctionApp.Extensions;
using CT.Gadgets.FunctionApp.Helpers;
using CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;
using CT.Gadgets.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using static CT.Application.FeaturesIS.Login.Queries.RefreshLoginQuery;

namespace CT.Gadgets.FunctionApp.Functions.Authentication;

public class RefreshLogin(ILogger<RefreshLogin> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<RefreshLogin> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = FeatureGlobals.Version1;
    private const string Tag = FeatureGlobals.Authentication.Tag;
    private const string OperationId = $"{Tag}-refresh";

    [EnableCors]
    [Function(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<RefreshLoginQueryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}/refresh")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var query = new RefreshLoginQuery();

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
            , _logger).ConfigureAwait(false);

        return response;
    }
}