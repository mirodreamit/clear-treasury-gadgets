using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Exceptions;
using CT.Application.Abstractions.Models;
using CT.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace CT.Application.Behaviors;

public class UnhandledExceptionsBehavior<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (UserFriendlyException friendlyException)
        {
            return CreateResponse(OperationResult.BadRequest, HttpStatusCode.BadRequest, friendlyException.Message);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            var sb = new StringBuilder();
            var current = ex;
            while (current != null)
            {
                sb.AppendLine($"{current.GetType().Name}: {current.Message}");
                current = current.InnerException;
            }

            _logger.LogError(ex,
                "Unhandled exception in {RequestName}. Errors: {Errors}",
                requestName, sb.ToString());

            var userMessage = "Something went wrong. Please try again later.";
            
            return CreateResponse(OperationResult.InternalError, HttpStatusCode.InternalServerError, userMessage);
        }
    }
    private static TResponse CreateResponse(OperationResult result, HttpStatusCode statusCode, string userMessage)
    {
        var ctor = typeof(TResponse).GetConstructor([
            typeof(OperationResult),
                typeof(string),
                typeof(ApplicationError)
        ]);

        if (ctor != null)
        {
            return (TResponse)ctor.Invoke(
            [
                    result,
                    statusCode.ToString(),
                    new ApplicationError(userMessage)
            ]);
        }

        throw new InvalidOperationException(
            $"Cannot construct {typeof(TResponse).Name}. " +
            "Expected a constructor with (OperationResult, int, ApplicationError).");
    }
}
