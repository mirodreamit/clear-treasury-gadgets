using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;

public class SortOpenApiParameterAttribute : OpenApiParameterAttribute
{
    public const string ParameterName = "sort";

    public SortOpenApiParameterAttribute() : base(ParameterName)
    {
        In = ParameterLocation.Query;
        Required = false;
        Type = typeof(string);
        Description = "[{\"fieldName\":\"fieldName\",\"direction\":\"asc|desc\"}]";
    }
}
