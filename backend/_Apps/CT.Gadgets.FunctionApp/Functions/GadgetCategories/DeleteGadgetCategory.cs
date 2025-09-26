using CT.Application.Abstractions.Models;
using CT.Application.Features.GadgetCategories.Commands;
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
using static CT.Application.Features.GadgetCategories.Commands.DeleteGadgetCategoryCommand;


namespace CT.Gadgets.FunctionApp.Functions.GadgetCategories;

public class DeleteGadgetCategory(ILogger<DeleteGadgetCategory> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<DeleteGadgetCategory> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = $"{FeatureGlobals.GadgetCategories.Tag}-delete";
    
    #pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [Function(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [FeatureGlobals.GadgetCategories.Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<DeleteGadgetCategoryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{FeatureGlobals.Version1}/{FeatureGlobals.GadgetCategories.Tag}/{{id}}")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var cmd = new DeleteGadgetCategoryCommand(id);

        var response = 
            await _httpRequestProcessingService.ProcessHttpRequestAsync(async () => 
                await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false), 
                _logger)
            .ConfigureAwait(false);

        return response;
    }
}