using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CT.Application.Abstractions.Interfaces;
using CT.Application.Configuration;
using CT.Application.Extensions;
using CT.Application.Models;
using CT.Tests.Configuration;
using TestHelper = CT.Tests.Configuration.Helpers;
using CT.Tests.Services;

namespace CT.Tests.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        var config = TestHelper.InitConfiguration();

        var testProjectConfigurationSection = config.GetSection("TestProjectConfiguration");

        services.Configure<TestProjectConfiguration>(testProjectConfigurationSection);
        services.AddScoped(cfg => cfg.GetRequiredService<IOptions<TestProjectConfiguration>>().Value);

        services.Configure<ApplicationConfiguration>(config.GetSection("ApplicationConfiguration"));
        services.AddSingleton(cfg => cfg.GetRequiredService<IOptions<ApplicationConfiguration>>().Value);

        var provider = services.BuildServiceProvider();
        
        var testProjectConfig = provider.GetRequiredService<TestProjectConfiguration>();
        
        services.AddApplication(testProjectConfig.CnnStr);
        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();

        var sp = services.BuildServiceProvider();
        
        var testProjectConfiguration = sp.GetService<TestProjectConfiguration>();
        var applicationConfiguration = sp.GetService<ApplicationConfiguration>();
        
        services.AddLogging(config => config.AddDebug());
                
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MaxDepth = 128 };

        return services;
    }
}
