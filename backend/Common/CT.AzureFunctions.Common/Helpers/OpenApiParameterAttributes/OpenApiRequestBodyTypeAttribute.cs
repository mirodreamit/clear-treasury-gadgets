using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System;

namespace CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;

public class OpenApiRequestBodyTypeAttribute : OpenApiRequestBodyAttribute
{
    public OpenApiRequestBodyTypeAttribute(Type bodyType, bool required) : base(contentType: "application/json", bodyType: bodyType)
    {
        Description = "Data in json format.";
        Required = required;
    }
}
