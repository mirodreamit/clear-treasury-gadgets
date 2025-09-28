using CT.Application.Abstractions.Models;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Features.Categories.Queries;
using CT.Gadgets.FunctionApp.Extensions;
using CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;
using CT.Gadgets.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace CT.Gadgets.FunctionApp.Functions.Categories;

public class CategoryQueries(ILogger<CategoryQueries> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<CategoryQueries> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = "v1";
    private const string Tag = "categories";

    [EnableCors]
    [Function($"{Tag}-get")]
    [OpenApiOperation(operationId: $"{Tag}-get", tags: [Tag])]
    [FilterOpenApiParameter]
    [SortOpenApiParameter]
    [PagingOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(GetEntitiesResponse<GetCategoriesQueryResponseModel>))]
    public async Task<HttpResponseData> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Version}/{Tag}")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var filterParameters = req.GetQueryParameter<FilterQueryParameters>(FilterOpenApiParameterAttribute.ParameterName);
            var sortParameters = req.GetQueryParameter<SortQueryParameters>(SortOpenApiParameterAttribute.ParameterName); 
            var pagingParameters = req.GetQueryParameter<PagingQueryParameters>(PagingOpenApiParameterAttribute.ParameterName); 

            var query = new GetCategoriesQuery()
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
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<GetCategoryByIdQueryResponseModel>))]
    public async Task<HttpResponseData> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var query = new GetCategoryByIdQuery(id);

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

