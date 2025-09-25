using CT.Application.Abstractions.Models;
using CT.Application.FeaturesIS.Login.Queries;
using CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;
using CT.FunctionApp.Helpers;
using CT.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using CT.AzureFunctions.Common.Extensions;

using static CT.Application.FeaturesIS.Login.Queries.RefreshLoginQuery;

namespace CT.FunctionApp.FunctionsIS.Authentication;

public class RefreshLogin(ILogger<RefreshLogin> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<RefreshLogin> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "authentication-refresh";
    private const string Tag = "authentication";
    private const string Version = "v1";

    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [OpenApiRequestBodyType(typeof(RefreshLoginQueryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<RefreshLoginQueryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}/refresh")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<RefreshLoginQueryRequestModel>();

            var query = new RefreshLoginQuery(model);

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
            , _logger).ConfigureAwait(false);

        return response;
    }
}