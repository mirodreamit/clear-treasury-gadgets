using CT.Application.Abstractions.Models;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Features.GadgetCategories.Queries;
using CT.Gadgets.FunctionApp.Extensions;
using CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;
using CT.Gadgets.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace CT.Gadgets.FunctionApp.Functions.GadgetCategories;

public class GadgetCategoriesQueries(ILogger<GadgetCategoriesQueries> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<GadgetCategoriesQueries> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = "v1";
    private const string Tag = "gadget-categories";
        
    [EnableCors]
    [Function($"{Tag}-get")]
    [OpenApiOperation(operationId: $"{Tag}-get", tags: [Tag])]
    [FilterOpenApiParameter]
    [SortOpenApiParameter]
    [PagingOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(GetEntitiesResponse<GetGadgetCategoriesQueryResponseModel>))]
    public async Task<HttpResponseData> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Version}/{Tag}")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var filterParameters = req.GetQueryParameter<FilterQueryParameters>(FilterOpenApiParameterAttribute.ParameterName);
            var sortParameters = req.GetQueryParameter<SortQueryParameters>(SortOpenApiParameterAttribute.ParameterName);
            var pagingParameters = req.GetQueryParameter<PagingQueryParameters>(PagingOpenApiParameterAttribute.ParameterName);

            var query = new GetGadgetCategoriesQuery()
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

    
    #pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [Function($"{Tag}-get-by-id")]
    [OpenApiOperation(operationId: $"{Tag}-get-by-id", tags: [Tag])]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<GetGadgetCategoryByIdQueryResponseModel>))]
    public async Task<HttpResponseData> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var query = new GetGadgetCategoryByIdQuery(id);

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}