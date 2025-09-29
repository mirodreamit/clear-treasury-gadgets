using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;

public class BearerTokenOpenApiSecurityAttribute : OpenApiSecurityAttribute
{
    public const string BearerTokenHeader = "Authorization";

    public BearerTokenOpenApiSecurityAttribute() : base(BearerTokenHeader, SecuritySchemeType.ApiKey)
    {
        Name = BearerTokenHeader;
        In = OpenApiSecurityLocationType.Header;
        Scheme = OpenApiSecuritySchemeType.Bearer;
        BearerFormat = "JWT";
        Description = "Please enter a valid token (include the prefix Bearer)";
    }
}
