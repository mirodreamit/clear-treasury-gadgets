using CT.Application.Abstractions.Models;
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
using static CT.Application.Features.Gadgets.Commands.UpsertGadgetCommand;

using CT.Application.Features.Gadgets.Commands;

namespace CT.Gadgets.FunctionApp.Functions.Gadgets;

public class IncreaseGadgetStockQuantity(ILogger<IncreaseGadgetStockQuantity> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<IncreaseGadgetStockQuantity> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = FeatureGlobals.Version1;
    private const string Tag = FeatureGlobals.Gadgets.Tag;
    private const string OperationId = $"{Tag}-increase-stock";

    [EnableCors]
    [Function(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = $"{Version}/{Tag}/{{id}}/increase-stock-by-one")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var cmd = new IncreaseGadgetStockQuantityCommand(id);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

