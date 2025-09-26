using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System;

namespace CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;

public class IdOpenApiParameterAttribute : OpenApiParameterAttribute
{
    public const string ParameterName = "id";
    public IdOpenApiParameterAttribute() : base(ParameterName)
    {
        In = ParameterLocation.Path;
        Required = true;
        Type = typeof(Guid);
        Description = "Id";
    }
}
