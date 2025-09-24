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

namespace CT.Tests.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestServices(this IServiceCollection serviceCollection)
    {
        var config = TestHelper.InitConfiguration();

        var testProjectConfigurationSection = config.GetSection("TestProjectConfiguration");

        serviceCollection.Configure<TestProjectConfiguration>(testProjectConfigurationSection);
        serviceCollection.AddScoped(cfg => cfg.GetRequiredService<IOptions<TestProjectConfiguration>>().Value);

        serviceCollection.Configure<ApplicationConfiguration>(config.GetSection("ApplicationConfiguration"));
        serviceCollection.AddSingleton(cfg => cfg.GetRequiredService<IOptions<ApplicationConfiguration>>().Value);

        var provider = serviceCollection.BuildServiceProvider();
        
        var testProjectConfig = provider.GetRequiredService<TestProjectConfiguration>();
        
        serviceCollection.AddQMApplication(testProjectConfig.CnnStr);

        var sp = serviceCollection.BuildServiceProvider();
        
        var testProjectConfiguration = sp.GetService<TestProjectConfiguration>();
        var applicationConfiguration = sp.GetService<ApplicationConfiguration>();
        
        serviceCollection.AddLogging(config => config.AddDebug());
                
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MaxDepth = 128 };

        return serviceCollection;
    }
}
