using CT.Application.Abstractions.Models;
using CT.Application.Features.Gadgets.Commands;
using CT.Application.FeaturesIS.Login.Queries;
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
using static CT.Application.Features.Gadgets.Commands.CreateGadgetFullCommand;

namespace CT.Gadgets.FunctionApp.Functions.Gadgets;

public class PostGadgetFull(ILogger<PostGadgetFull> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<PostGadgetFull> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = FeatureGlobals.Version1;
    private const string Tag = FeatureGlobals.Gadgets.Tag;
    private const string OperationId = $"{Tag}-create-full";

    [EnableCors]
    [Function(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [OpenApiRequestBodyType(typeof(CreateGadgetFullRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<CreateGadgetFullResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}/with-categories")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetFullRequestModel>();

            var cmd = new CreateGadgetFullCommand(Guid.NewGuid(), model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}