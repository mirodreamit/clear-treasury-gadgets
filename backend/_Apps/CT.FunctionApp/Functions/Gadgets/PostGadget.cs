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
using Microsoft.OpenApi.Models;
using CT.Application.Abstractions.Models;
using CT.Application.Features.Gadgets.Commands;
using CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;
using CT.FunctionApp.Helpers;
using CT.AzureFunctions.Common.Extensions;

using static CT.Application.Features.Gadgets.Commands.UpsertGadgetCommand;
using CT.FunctionApp.Interfaces;

namespace CT.FunctionApp.Functions.Gadgets;

public class PostGadget(ILogger<PostGadget> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<PostGadget> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "gadgets-create";
    private const string Tag = "gadgets";
    private const string Version = "v1";

    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [OpenApiRequestBodyType(typeof(CreateGadgetRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetRequestModel>();

            var cmd = new UpsertGadgetCommand(Guid.NewGuid(), model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

