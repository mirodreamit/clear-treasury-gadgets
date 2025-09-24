using Microsoft.Extensions.DependencyInjection;
using CT.FunctionApp.Services;
using CT.FunctionApp.Interfaces;

namespace CT.FunctionApp.Extensions;

internal static class ConfigureServicesExtensions
{
    internal static IServiceCollection AddQMFuntionApp(this IServiceCollection services, string rootAppDirectory)
    {
        services.AddScoped<IHttpRequestProcessingService, HttpRequestProcessingService>();
        
        return services;
    }
}