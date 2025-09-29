using CT.Application.Abstractions.Models;
using CT.Application.Features.Authentication.Queries;
using CT.Gadgets.FunctionApp.Extensions;
using CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;
using CT.Gadgets.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using static CT.Application.Features.Authentication.Queries.BasicLoginUserQuery;
using static CT.Application.Features.Authentication.Queries.RefreshLoginQuery;
using CT.Gadgets.FunctionApp.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CT.Gadgets.FunctionApp.Functions.Authentication;

public class AuthenticationQueries(ILogger<AuthenticationQueries> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<AuthenticationQueries> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = "v1";
    private const string Tag = "auth";
    
    [EnableCors]
    [Function($"{Tag}-login")]
    [OpenApiOperation(operationId: $"{Tag}-login", tags: [Tag])]
    [OpenApiRequestBodyType(typeof(BasicLoginUserQueryRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<BasicLoginUserQueryResponseModel>))]
    public async Task<HttpResponseData> BasicLoginAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}/login")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        string refreshToken = null!;

        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<BasicLoginUserQueryRequestModel>();

            var query = new BasicLoginUserQuery(model);

            var responseModel = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

            if (responseModel.Result == Application.Abstractions.Enums.OperationResult.Ok)
            {
                refreshToken = responseModel!.Model!.RefreshToken!;
            }

            return responseModel;

        }
            , _logger).ConfigureAwait(false);

        if (response is HttpResponseData httpResponse)
        {
            httpResponse.SetRefreshTokenCookie(refreshToken);

            return httpResponse;
        }

        return response;
    }

    [EnableCors]
    [Function($"{Tag}-refresh")]
    [OpenApiOperation(operationId: $"{Tag}-refresh", tags: [Tag])]
    [RefreshTokenCookieParameter]
    [Authorize]
    [BearerTokenOpenApiSecurity]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<RefreshLoginQueryResponseModel>))]
    public async Task<HttpResponseData> RefreshLogin(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Version}/{Tag}/refresh")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        string refreshToken = null!;
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var query = new RefreshLoginQuery();

            var responseModel = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

            if (responseModel.Result == Application.Abstractions.Enums.OperationResult.Ok)
            {
                refreshToken = responseModel!.Model!.RefreshToken!;
            }

            return responseModel;

        }
            , _logger).ConfigureAwait(false);

        if (response is HttpResponseData httpResponse)
        {
            httpResponse.SetRefreshTokenCookie(refreshToken);
            
            return httpResponse;
        }

        return response;
    }
}