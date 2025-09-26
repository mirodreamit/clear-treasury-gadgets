using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System;

namespace CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;

public class QueryOpenApiParameterAttribute : OpenApiParameterAttribute
{
    public QueryOpenApiParameterAttribute(string parameterName, string description, bool required, Type type) : base(parameterName)
    {
        In = ParameterLocation.Query;
        Required = required;
        Type = type;
        Description = description;
    }
}
