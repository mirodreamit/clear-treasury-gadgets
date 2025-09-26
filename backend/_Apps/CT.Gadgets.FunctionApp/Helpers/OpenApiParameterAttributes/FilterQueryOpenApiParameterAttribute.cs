using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System;

namespace CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;

public class FilterOpenApiParameterAttribute : OpenApiParameterAttribute
{
    public const string ParameterName = "filter";

    public FilterOpenApiParameterAttribute() : base(ParameterName)
    {
        In = ParameterLocation.Query;
        Required = false;
        Type = typeof(string);
        Description = "[{\"fieldName\":\"fieldName\",\"filter\":[{\"op\":\"gt|lt|gte|lte|eq|startsWith|contains\",\"value\":\"some_value\"}]}]";
    }
}
