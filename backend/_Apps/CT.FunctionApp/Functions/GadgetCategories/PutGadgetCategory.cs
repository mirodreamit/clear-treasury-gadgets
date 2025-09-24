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
using CT.Application.Features.GadgetCategories.Commands;
using CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;
using CT.FunctionApp.Helpers;
using CT.AzureFunctions.Common.Extensions;

using static CT.Application.Features.GadgetCategories.Commands.UpsertGadgetCategoryCommand;
using CT.FunctionApp.Interfaces;

namespace CT.FunctionApp.Functions.GadgetCategories;

public class PutGadgetCategory(ILogger<PutGadgetCategory> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<PutGadgetCategory> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "gadget-categories-update";
    private const string Tag = "gadget-categories";
    private const string Version = "v1";

    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OpenApiRequestBodyType(typeof(CreateGadgetCategoryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetCategoryResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}/{{id}}")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetCategoryRequestModel>();

            var cmd = new UpsertGadgetCategoryCommand(id, model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

