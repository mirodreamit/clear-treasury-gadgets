using System.Net;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Text;
using CT.Application.Models;
using CT.Application.Abstractions.Enums;

namespace CT.Application.Behaviors;

public class UnhandledExceptionsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionsBehavior(ILogger<TRequest> logger) => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
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

            var ctor = typeof(TResponse).GetConstructor([
                typeof(OperationResult),
                typeof(string),
                typeof(ApplicationError)
            ]);

            if (ctor != null)
            {
                return (TResponse)ctor.Invoke(
                [
                    OperationResult.InternalError,
                    HttpStatusCode.InternalServerError.ToString(),
                    new ApplicationError(userMessage)
                ]);
            }

            throw new InvalidOperationException(
                $"Cannot construct {typeof(TResponse).Name}. " +
                "Expected a constructor with (OperationResult, int, ApplicationError).");
        }
    }
}
