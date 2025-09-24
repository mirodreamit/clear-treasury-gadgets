using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;
using CT.AzureFunctions.Common.Extensions;
using CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;
using CT.FunctionApp.Helpers;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Abstractions.Models;
using System.Threading;
using System;
using CT.Application.Features.Gadgets.Queries;
using CT.FunctionApp.Interfaces;

namespace CT.FunctionApp.Functions.Gadgets;

public class GetGadgetFullByGadgetId(ILogger<GetGadgetFullByGadgetId> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<GetGadgetFullByGadgetId> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "gadgets-get-full-by-id";
    private const string Tag = "gadgets";
    private const string Version = "v1";

    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [SortOpenApiParameter]
    [PagingOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<GetGadgetFullByIdQueryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}/{{id}}/with-categories")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var sortParameters = req.GetQueryParameter<SortQueryParameters>(SortOpenApiParameterAttribute.ParameterName);
            var pagingParameters = req.GetQueryParameter<PagingQueryParameters>(PagingOpenApiParameterAttribute.ParameterName);

            var query = new GetGadgetFullByIdQuery(id)
            {
                SortParameters = sortParameters,
                PagingParameters = pagingParameters
            };

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

