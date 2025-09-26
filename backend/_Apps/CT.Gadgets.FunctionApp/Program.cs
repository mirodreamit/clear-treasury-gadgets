using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker;
using CT.Application.Configuration;
using CT.Application.Extensions;
using CT.Gadgets.FunctionApp.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using CT.Gadgets.FunctionApp.Middlewares;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<ApplicationConfiguration>(
    builder.Configuration.GetSection("ApplicationConfiguration"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<ApplicationConfiguration>>().Value);

var dbConnectionString = builder.Configuration.GetConnectionString("Gadgets")!;

builder.ConfigureFunctionsWebApplication();

builder.UseMiddleware<HttpContextMiddleware>();

builder.Services.AddFunctionApp();
builder.Services.AddApplication(dbConnectionString);

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();


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