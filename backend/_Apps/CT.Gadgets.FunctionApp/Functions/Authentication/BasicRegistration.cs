using CT.Application.Abstractions.Models;
using CT.Application.FeaturesIS.Register.Commands;
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
using static CT.Application.FeaturesIS.Register.Commands.BasicRegisterUserCommand;

namespace CT.Gadgets.FunctionApp.Functions.Authentication;

public class BasicRegister(ILogger<BasicRegister> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<BasicRegister> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string Version = FeatureGlobals.Version1;
    private const string Tag = FeatureGlobals.Authentication.Tag;
    private const string OperationId = $"{Tag}-signup";

    [EnableCors]
    [Function(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [OpenApiRequestBodyType(typeof(BasicRegisterUserCommandRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<BasicRegisterUserCommandResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Version}/{Tag}/signup")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var response = await _httpRequestProcessingService.ProcessHttpRequestAsync(async () =>
        {
            var model = await req.GetModelFromRequestBodyAsync<BasicRegisterUserCommandRequestModel>();

            var query = new BasicRegisterUserCommand(model);

            return await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        }
            , _logger).ConfigureAwait(false);

        return response;
    }
}