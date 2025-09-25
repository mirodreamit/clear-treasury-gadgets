using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MediatR;
using CT.Application.Configuration;

namespace CT.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger, ApplicationConfiguration config) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;
    private readonly ApplicationConfiguration _config = config;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_config == null)
        {
            throw new NullReferenceException("ApplicationConfiguration cannot be null. Check Dependency Injection.");
        }

        if (_logger == null)
        {
            throw new NullReferenceException("ILogger<TRequest> cannot be null. Check Dependency Injection.");
        }

        TResponse response;

        var timer = new Stopwatch();

        try
        {
            timer.Start();

            response = await next();
        }
        finally
        {
            timer.Stop();
            try
            {
                if (timer.ElapsedMilliseconds > _config!.RequestProcessingConfiguration!.WarningThresholdMiliseconds)
                {
                    var name = typeof(TRequest).Name;
                    string msg = $"Long Running Request: [RequestName: {name}] [Elapsed Miliseconds: {timer.ElapsedMilliseconds}]";
                    
                    _logger.LogWarning("{Message}", msg);
                }
            }
            catch (Exception)
            {

            }
        }

        return response;
    }
}