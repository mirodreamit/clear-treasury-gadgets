using CT.Application.Abstractions.Interfaces;
using CT.Application.Interfaces;
using CT.Gadgets.FunctionApp.Interfaces;
using CT.Gadgets.FunctionApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CT.Gadgets.FunctionApp.Extensions;

internal static class ConfigureServicesExtensions
{
    internal static IServiceCollection AddFunctionApp(this IServiceCollection services, string gadgetsHubBaseAddress)
    {
        services.AddScoped<IHttpRequestProcessingService, HttpRequestProcessingService>();
        services.AddScoped<HttpRequestContextAccessor>();
        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();

        services.AddHttpClient<GadgetsHubHttpClient>(client =>
        {
            client.BaseAddress = new Uri(gadgetsHubBaseAddress); 
        });
        services.AddSingleton<IGadgetNotifier, GadgetNotifier>();

        return services;
    }
}