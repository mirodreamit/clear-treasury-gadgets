using CT.Application.Abstractions.Models;
using CT.Application.Features.GadgetCategories.Commands;
using CT.Gadgets.FunctionApp.Extensions;
using CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;
using CT.Gadgets.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using static CT.Application.Features.GadgetCategories.Commands.DeleteGadgetCategoryCommand;
using static CT.Application.Features.GadgetCategories.Commands.UpsertGadgetCategoryCommand;


namespace CT.Gadgets.FunctionApp.Functions.GadgetCategories;

public class GadgetCategoryCommands(ILogger<GadgetCategoryCommands> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<GadgetCategoryCommands> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = "v1";
    private const string Tag = "gadget-categories";

    #pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [Function($"{Tag}-delete")]
    [OpenApiOperation(operationId: $"{Tag}-delete", tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<DeleteGadgetCategoryResponseModel>))]
    public async Task<HttpResponseData> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var cmd = new DeleteGadgetCategoryCommand(id);

        var response = 
            await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () => 
                await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false), 
                _logger)
            .ConfigureAwait(false);

        return response;
    }

    [EnableCors]
    [Function($"{Tag}-create")]
    [OpenApiOperation(operationId: $"{Tag}-create", tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [OpenApiRequestBodyType(typeof(CreateGadgetCategoryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetCategoryResponseModel>))]
    public async Task<HttpResponseData> Post(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetCategoryRequestModel>();

            var cmd = new UpsertGadgetCategoryCommand(Guid.NewGuid(), model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }

    [EnableCors]
    [Function($"{Tag}-update")]
    [OpenApiOperation(operationId: $"{Tag}-update", tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OpenApiRequestBodyType(typeof(CreateGadgetCategoryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetCategoryResponseModel>))]
    public async Task<HttpResponseData> Put(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetCategoryRequestModel>();

            var cmd = new UpsertGadgetCategoryCommand(id, model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}