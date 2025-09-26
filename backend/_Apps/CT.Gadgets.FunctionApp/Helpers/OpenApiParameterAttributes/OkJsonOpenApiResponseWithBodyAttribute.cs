using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System;

namespace CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;

public class OkJsonOpenApiResponseWithBodyAttribute : OpenApiResponseWithBodyAttribute
{
    public OkJsonOpenApiResponseWithBodyAttribute() : this(typeof(string))
    {

    }

    public OkJsonOpenApiResponseWithBodyAttribute(Type bodyType) : base(System.Net.HttpStatusCode.OK, "application/json", bodyType)
    {
        Description = "The OK response";
    }
}
