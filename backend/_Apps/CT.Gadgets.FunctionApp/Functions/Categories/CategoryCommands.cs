using CT.Application.Abstractions.Models;
using CT.Application.Features.Categories.Commands;
using CT.Gadgets.FunctionApp.Extensions;
using CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;
using CT.Gadgets.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using static CT.Application.Features.Categories.Commands.DeleteCategoryCommand;
using static CT.Application.Features.Categories.Commands.UpsertCategoryCommand;

namespace CT.Gadgets.FunctionApp.Functions.Categories;

public class CategoryCommands(ILogger<CategoryCommands> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<CategoryCommands> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = "v1";
    private const string Tag = "categories";
    
    [EnableCors]
    [Function($"{Tag}-delete")]
    [OpenApiOperation(operationId: $"{Tag}-delete", tags: [Tag])]
    [BearerTokenOpenApiSecurity]
    [IdOpenApiParameter]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<DeleteCategoryResponseModel>))]
    public async Task<HttpResponseData> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var cmd = new DeleteCategoryCommand(id);
        
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
    [OpenApiRequestBodyType(typeof(CreateCategoryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertCategoryResponseModel>))]
    public async Task<HttpResponseData> Post(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateCategoryRequestModel>();

            var cmd = new UpsertCategoryCommand(Guid.NewGuid(), model);

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
    [OpenApiRequestBodyType(typeof(CreateCategoryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<UpsertCategoryResponseModel>))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = $"{Version}/{Tag}/{{id}}")] HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<CreateCategoryRequestModel>();

            var cmd = new UpsertCategoryCommand(id, model);

            return await _mediator.Send(cmd, cancellationToken).ConfigureAwait(false);
        }
        , _logger).ConfigureAwait(false);

        return response;
    }
    #pragma warning restore IDE0060 // Remove unused parameter
}