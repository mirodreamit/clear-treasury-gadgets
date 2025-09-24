using Microsoft.Extensions.Configuration;
using System;

namespace CT.AzureFunctions.Common.Helpers;

public static class EnvironmentHelpers
{
    public static PlatformType GetPlatformType()
    {
        PlatformType platformType = PlatformType.Local;

        string envValue = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_PLATFORM")!;

        if ("Azure".Equals(envValue, StringComparison.OrdinalIgnoreCase))
        {
            platformType = PlatformType.Azure;
        }

        return platformType;
    }

    public static EnvironmentType GetEnvironmentType()
    {
        EnvironmentType environmentType = EnvironmentType.Development;

        string envValue = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT_OVERRIDE")!;

        envValue ??= Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")!;

        if ("Sit".Equals(envValue, StringComparison.OrdinalIgnoreCase))
        {
            environmentType = EnvironmentType.Sit;
        }
        else if ("Uat".Equals(envValue, StringComparison.OrdinalIgnoreCase))
        {
            environmentType = EnvironmentType.Uat;
        }
        else if ("Production".Equals(envValue, StringComparison.OrdinalIgnoreCase))
        {
            environmentType = EnvironmentType.Production;
        }

        return environmentType;
    }

    #region private methods

    public static string GetCurrentDirectory()
    {
        var platformType = GetPlatformType();

        var local_root = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot")!;
        var azure_root = $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";

        string currentDirectory = platformType switch
        {
            PlatformType.Local => local_root,
            PlatformType.Azure => azure_root,
            _ => local_root,
        };

        return currentDirectory;
    }


    public static void LoadSettings(IConfigurationBuilder configuration)
    {
        EnvironmentType envType = GetEnvironmentType();

        if (envType == EnvironmentType.Development)
        {
            configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);
            configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        }
        else if (envType == EnvironmentType.Sit)
        {
            configuration.AddJsonFile("appsettings.sit.json", optional: true, reloadOnChange: false);
        }
        else if (envType == EnvironmentType.Uat)
        {
            configuration.AddJsonFile("appsettings.uat.json", optional: true, reloadOnChange: false);
        }
        else if (envType == EnvironmentType.Production)
        {
            configuration.AddJsonFile("appsettings.prod.json", optional: true, reloadOnChange: false);
        }
    }


    #endregion
}
