using CT.Application.Abstractions.Models;
using CT.Application.Features.Gadgets.Commands;
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
using System;
using System.Threading;
using System.Threading.Tasks;
using static CT.Application.Features.Gadgets.Commands.DeleteGadgetFullCommand;

namespace CT.FunctionApp.Functions.Gadgets;

public class DeleteGadgetFull(ILogger<DeleteGadgetFull> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<DeleteGadgetFull> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "gadgets-delete-full";
    private const string Tag = "gadgets";
    private const string Version = "v1";

    #pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<DeleteGadgetFullResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}/{{id}}/with-categories")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var cmd = new DeleteGadgetFullCommand(id);
        
        var response = 
            await _httpRequestProcessingService.ProcessHttpRequestAsync(async () => 
                await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false), 
                _logger)
            .ConfigureAwait(false);

        return response;
    }
}