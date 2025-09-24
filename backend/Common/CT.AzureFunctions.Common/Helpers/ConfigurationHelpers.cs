using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace CT.AzureFunctions.Common.Helpers;

public static class ConfigurationHelpers
{
    public static IConfigurationRoot GetConfig(IFunctionsHostBuilder builder)
    {
        string currentDirectory = EnvironmentHelpers.GetCurrentDirectory();

        var configurationBuilder = new ConfigurationBuilder();

        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));

        if (descriptor?.ImplementationInstance is IConfiguration configRoot)
        {
            configurationBuilder.AddConfiguration(configRoot);
        }

        IConfigurationBuilder configuration = configurationBuilder
            .AddEnvironmentVariables()
            .SetBasePath(currentDirectory);

        EnvironmentHelpers.LoadSettings(configuration);

        IConfigurationRoot config = configuration.Build();

        return config;
    }

}
