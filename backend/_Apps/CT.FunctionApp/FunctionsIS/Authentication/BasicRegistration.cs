using CT.Application.Abstractions.Models;
using CT.Application.FeaturesIS.Register.Commands;
using CT.AzureFunctions.Common.Extensions;
using CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;
using CT.FunctionApp.Helpers;
using CT.FunctionApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using static CT.Application.FeaturesIS.Register.Commands.BasicRegisterUserCommand;

namespace CT.FunctionApp.FunctionsIS.Authentication;

public class BasicRegister(ILogger<BasicRegister> log, IMediator mediator, IHttpRequestProcessingService httpRequestProcessingService)
{
    private readonly ILogger<BasicRegister> _logger = log;
    private readonly IMediator _mediator = mediator;
    private readonly IHttpRequestProcessingService _httpRequestProcessingService = httpRequestProcessingService;

    private const string OperationId = "authentication-signup";
    private const string Tag = "authentication";
    private const string Version = "v1";

    [EnableCors]
    [FunctionName(OperationId)]
    [OpenApiOperation(operationId: OperationId, tags: [Tag])]
    [OpenApiRequestBodyType(typeof(BasicRegisterUserCommandRequestModel), true)]
    [OkJsonOpenApiResponseWithBody(typeof(BaseOutput<BasicRegisterUserCommandResponseModel>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{Constants.ApiDomainName}/{Version}/{Tag}/signup")] HttpRequest req,
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