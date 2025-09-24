using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace CT.AzureFunctions.Common.Helpers.OpenApiParameterAttributes;

public class PagingOpenApiParameterAttribute : OpenApiParameterAttribute
{
    public const string ParameterName = "paging";

    public PagingOpenApiParameterAttribute() : base(ParameterName)
    {
        In = ParameterLocation.Query;
        Required = false;
        Type = typeof(string);
        Description = "{\"pageSize\":\"-1\",\"pageIndex\":\"0\"}";
    }
}
