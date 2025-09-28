using CT.Application.Constants;
using CT.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CT.Gadgets.FunctionApp.Services;

public abstract class BaseContextAccessor
{
    private readonly HttpRequestContextAccessor _httpContextAccessor;
    private readonly ITokenGenerator _tokenGenerator;

    public BaseContextAccessor(HttpRequestContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenGenerator = tokenGenerator;        
    }

    protected string? GetHeaderValue(string header)
    {
        return GetHeaderValues(header)?.FirstOrDefault();
    }

    protected List<string?> GetHeaderValues(string headerKey)
    {
        var headerValues = new List<string?>();

        if (_httpContextAccessor.HttpRequestData is not null)
        {
            var requestHeaders = _httpContextAccessor.HttpRequestData.Headers;

            if (requestHeaders.TryGetValues(headerKey, out var values))
            {
                headerValues.AddRange(values);
            }
        }

        return headerValues;
    }

    public string? GetUserIdentifierFromCookie()
    {
        var headers = _httpContextAccessor?.HttpRequestData?.Headers;

        if (headers is not null && headers.TryGetValues("Cookie", out var cookieHeaders))
        {
            var cookie = cookieHeaders.FirstOrDefault();
            var refreshToken = cookie?
                .Split(';')
                .Select(c => c.Trim())
                .FirstOrDefault(c => c.StartsWith("refreshToken="))
                ?.Split('=')[1];

            if (_tokenGenerator.ValidateToken(refreshToken!, out var claimsPrincipal, out Microsoft.IdentityModel.Tokens.SecurityToken? validatedToken))
            {
                return claimsPrincipal?.FindFirst(CustomClaimTypes.UserIdentifier)?.Value;
            }
        }

        return null;
    }

    
    protected string? GetClaimValueFromBearerToken(string claimType)
    {
        var authorizationHeaders = GetHeaderValues("authorization");

        if (authorizationHeaders?.Count > 0 == true)
        {
            var bearerToken = authorizationHeaders.First()!["Bearer ".Length..];

            if (_tokenGenerator.ValidateToken(bearerToken, out var claimsPrincipal, out Microsoft.IdentityModel.Tokens.SecurityToken? validatedToken))
            {
                return claimsPrincipal?.FindFirst(claimType)?.Value;
            }
        }

        return null;
    }
}
