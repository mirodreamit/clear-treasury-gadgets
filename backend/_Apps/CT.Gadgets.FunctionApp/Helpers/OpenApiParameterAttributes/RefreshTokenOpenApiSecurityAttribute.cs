using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace CT.Gadgets.FunctionApp.Helpers.OpenApiParameterAttributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RefreshTokenCookieParameterAttribute : OpenApiParameterAttribute
{
    public RefreshTokenCookieParameterAttribute() : base("refreshToken")
    {
        In = ParameterLocation.Cookie;
        Required = true;
        Type = typeof(string);
        Description = "HttpOnly cookie containing the refresh token";
    }
}