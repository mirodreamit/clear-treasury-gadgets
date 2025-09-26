using CT.Application.Abstractions.Models;
using CT.Application.Features.Categories.Commands;
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
using static CT.Application.Features.Categories.Commands.UpsertCategoryCommand;

namespace CT.Gadgets.FunctionApp.Functions.Categories;

public class PutCategory(ILogger<PutCategory> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<PutCategory> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = $"{FeatureGlobals.Categories.Tag}-update";
    
    [EnableCors]
    [Function(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [FeatureGlobals.Categories.Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OpenApiRequestBodyType(typeof(CreateCategoryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertCategoryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{FeatureGlobals.Version1}/{FeatureGlobals.Categories.Tag}/{{id}}")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateCategoryRequestModel>();

            var cmd = new UpsertCategoryCommand(id, model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

