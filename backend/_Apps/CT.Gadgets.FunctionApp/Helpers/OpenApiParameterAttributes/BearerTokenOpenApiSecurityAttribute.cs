using Microsoft.AspNetCore.Authorization;

namespace CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;

public class BearerTokenOpenApiSecurityAttribute : AuthorizeAttribute
{
    //public const string BearerTokenHeader = "Authorization";

    public BearerTokenOpenApiSecurityAttribute() : base()
    {
        //Name = BearerTokenHeader;
        //In = OpenApiSecurityLocationType.Header;
        //Scheme = OpenApiSecuritySchemeType.Bearer;
        //BearerFormat = "JWT";
        //Description = "Please enter a valid token (include the prefix Bearer)";
    }
}
