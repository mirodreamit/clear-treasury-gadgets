using System.Threading;
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

using CT.Application.Features.GadgetCategories.Queries;
using CT.FunctionApp.Interfaces;

namespace CT.FunctionApp.Functions.GadgetCategories;

public class GetGadgetCategories(ILogger<GetGadgetCategories> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<GetGadgetCategories> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "gadget-categories-get";
    private const string Tag = "gadget-categories";
    private const string Version = "v1";

    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [FilterOpenApiParameter]
    [SortOpenApiParameter]
    [PagingOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(GetEntitiesResponse<GetGadgetCategoriesQueryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var filterParameters = req.GetQueryParameter<FilterQueryParameters>(FilterOpenApiParameterAttribute.ParameterName);
            var sortParameters = req.GetQueryParameter<SortQueryParameters>(SortOpenApiParameterAttribute.ParameterName);
            var pagingParameters = req.GetQueryParameter<PagingQueryParameters>(PagingOpenApiParameterAttribute.ParameterName);

            var query = new GetGadgetCategoriesByGadgetIdQuery()
            {
                PagingParameters = pagingParameters,
                FilterParameters = filterParameters,
                SortParameters = sortParameters,
            };

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

