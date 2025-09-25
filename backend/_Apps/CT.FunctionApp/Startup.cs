using CT.Application.Configuration;
using CT.Application.Extensions;
using CT.AzureFunctions.Common.Helpers;
using CT.FunctionApp.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: FunctionsStartup(typeof(CT.FunctionApp.Startup))]

namespace CT.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfigurationRoot config = ConfigurationHelpers.GetConfig(builder);
            
            var dbConnectionString = config.GetConnectionString("Gadgets");

            builder.Services.AddLogging();
            
            builder.Services.Configure<ApplicationConfiguration>(config.GetSection("ApplicationConfiguration"));
            builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<ApplicationConfiguration>>().Value);

            var provider = builder.Services.BuildServiceProvider();
            var context = builder.GetContext();

            var functionAppDir = context.Configuration["AzureWebJobsScriptRoot"]
                            ?? context.Configuration["FUNCTIONS_WORKER_DIRECTORY"]
                            ?? AppContext.BaseDirectory;

            builder.Services.AddQMFuntionApp(functionAppDir);
            
            builder.Services.AddQMApplication(dbConnectionString);


            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                MaxDepth = 128,
            };
        }
    }

    public class OpenApiConfig : DefaultOpenApiConfigurationOptions
    {
        public OpenApiConfig() : base()
        {
            Info = new OpenApiInfo
            {
                Version = "v1",
                Title = "Clear Treasury - Gadgets API",
                Description = "API for managing gadgets in the Clear Treasure platform",
                Contact = new OpenApiContact
                {
                    Name = "CT Support",
                    Email = "support@ct.uk"
                }
            };

            Servers =
            [
                new() {
                    Url = "https://gadgets.ct.uk",
                    Description = "API for managing gadgets"
                }
            ];
        }

        public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;

        public override bool IncludeRequestingHostName { get; set; } = true;

    }
}
