using CT.Application.Abstractions.Models;
using CT.Application.Features.Authentication.Commands;
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
using static CT.Application.Features.Authentication.Commands.BasicRegisterUserCommand;

namespace CT.Gadgets.FunctionApp.Functions.Authentication;

public class AuthenticationCommands(ILogger<AuthenticationCommands> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<AuthenticationCommands> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = "v1";
    private const string Tag = "auth";
    
    [EnableCors]
    [Function($"{Tag}-signup")]
    [OpenApiOperation(operationId: $"{Tag}-signup", tags: [Tag])]
    [OpenApiRequestBodyType(typeof(BasicRegisterUserCommandRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<BasicRegisterUserCommandResponseModel>))]
    [ProducesErrorResponseType(typeof(BaseOutput<BasicRegisterUserCommandResponseModel>))]
    public async Task<HttpResponseData> BasicRegistration(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}/signup")] HttpRequestData req,
    CancellationToken cancellationToken)
    {
        string refreshToken = null!;
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(req, async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<BasicRegisterUserCommandRequestModel>();

            var query = new BasicRegisterUserCommand(model);

            var responseModel = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

            if (responseModel.Result == Application.Abstractions.Enums.OperationResult.Created)
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