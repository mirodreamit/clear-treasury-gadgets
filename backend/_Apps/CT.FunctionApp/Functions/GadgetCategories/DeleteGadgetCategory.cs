using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using CT.Application.Abstractions.Models;
using CT.Application.Features.GadgetCategories.Commands;
using CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;
using CT.FunctionApp.Helpers;

using static CT.Application.Features.GadgetCategories.Commands.DeleteGadgetCategoryCommand;
using CT.FunctionApp.Interfaces;

namespace CT.FunctionApp.Functions.GadgetCategories;

public class DeleteGadgetCategory(ILogger<DeleteGadgetCategory> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<DeleteGadgetCategory> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "gadget-categories-delete";
    private const string Tag = "gadget-categories";
    private const string Version = "v1";

    #pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<DeleteGadgetCategoryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}/{{id}}")] HttpRequest req,
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