using Microsoft.Extensions.DependencyInjection;
using CT.FunctionApp.Services;
using CT.FunctionApp.Interfaces;
using CT.Application.Abstractions.Interfaces;

namespace CT.FunctionApp.Extensions;

internal static class ConfigureServicesExtensions
{
    internal static IServiceCollection AddQMFuntionApp(this IServiceCollection services, string rootAppDirectory)
    {
        services.AddScoped<IHttpRequestProcessingService, HttpRequestProcessingService>();
        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();

        return services;
    }
}