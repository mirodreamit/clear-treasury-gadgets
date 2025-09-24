using CT.Application.Abstractions.Models;
using CT.Application.Features.Categories.Queries;
using CT.Application.Features.GadgetCategories.Queries;
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
using Microsoft.OpenApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CT.FunctionApp.Functions.GadgetCategories;

public class GetGadgetCategoryById(ILogger<GetGadgetCategoryById> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<GetGadgetCategoryById> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "gadget-categories-get-by-id";
    private const string Tag = "gadget-categories";
    private const string Version = "v1";


#pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<GetGadgetCategoryByIdQueryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}/{{id}}")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var query = new GetGadgetCategoryByIdQuery(id);

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

