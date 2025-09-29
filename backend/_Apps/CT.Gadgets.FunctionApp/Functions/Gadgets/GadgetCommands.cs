using CT.Application.Abstractions.Models;
using CT.Application.Features.Gadgets.Commands;
using CT.Gadgets.FunctionApp.Extensions;
using CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;
using CT.Gadgets.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using static CT.Application.Features.Gadgets.Commands.CreateGadgetFullCommand;
using static CT.Application.Features.Gadgets.Commands.DeleteGadgetCommand;
using static CT.Application.Features.Gadgets.Commands.DeleteGadgetFullCommand;
using static CT.Application.Features.Gadgets.Commands.UpsertGadgetCommand;

namespace CT.Gadgets.FunctionApp.Functions.Gadgets;

public class GadgetCommands(ILogger<GadgetCommands> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<GadgetCommands> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = "v1";
    private const string Tag = "gadgets";

    #pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [Function($"{Tag}-delete")]
    [OpenApiOperation(operationId: $"{Tag}-delete", tags: [Tag])]
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<DeleteGadgetResponseModel>))]
    public async Task<HttpResponseData> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var cmd = new DeleteGadgetCommand(id);

        var response =
            await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
                await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false),
                _logger)
            .ConfigureAwait(false);

        return response;
    }

    #pragma warning disable IDE0060 // Remove unused parameter
    [EnableCors]
    [Function($"{Tag}-delete-full")]
    [OpenApiOperation(operationId: $"{Tag}-delete-full", tags: [Tag])]
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<DeleteGadgetFullResponseModel>))]
    public async Task<HttpResponseData> DeleteFull(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{Version}/{Tag}/{{id}}/with-categories")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var cmd = new DeleteGadgetFullCommand(id);

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
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [OpenApiRequestBodyType(typeof(CreateGadgetRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetResponseModel>))]
    public async Task<HttpResponseData> Post(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetRequestModel>();

            var cmd = new UpsertGadgetCommand(Guid.NewGuid(), model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }

    [EnableCors]
    [Function($"{Tag}-create-full")]
    [OpenApiOperation(operationId: $"{Tag}-create-full", tags: [Tag])]
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [OpenApiRequestBodyType(typeof(CreateGadgetFullRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<CreateGadgetFullResponseModel>))]
    public async Task<HttpResponseData> PostFull(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}/with-categories")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetFullRequestModel>();

            var cmd = new CreateGadgetFullCommand(Guid.NewGuid(), model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }

    [EnableCors]
    [Function($"{Tag}-update")]
    [OpenApiOperation(operationId: $"{Tag}-update", tags: [Tag])]
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OpenApiRequestBodyType(typeof(CreateGadgetRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetResponseModel>))]
    public async Task<HttpResponseData> Put(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateGadgetRequestModel>();

            var cmd = new UpsertGadgetCommand(id, model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }

    [EnableCors]
    [Function($"{Tag}-increase-stock-by-one")]
    [OpenApiOperation(operationId: $"{Tag}-increase-stock-by-one", tags: [Tag])]
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetResponseModel>))]
    public async Task<HttpResponseData> IncreaseStockByOne(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = $"{Version}/{Tag}/{{id}}/increase-stock-by-one")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var cmd = new IncreaseGadgetStockQuantityCommand(id);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }

    [EnableCors]
    [Function($"{Tag}-decrease-stock-by-one")]
    [OpenApiOperation(operationId: $"{Tag}-decrease-stock-by-one", tags: [Tag])]
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertGadgetResponseModel>))]
    public async Task<HttpResponseData> DecreaseStockByOne(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = $"{Version}/{Tag}/{{id}}/decrease-stock-by-one")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var cmd = new DecreaseGadgetStockQuantityCommand(id);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
}

