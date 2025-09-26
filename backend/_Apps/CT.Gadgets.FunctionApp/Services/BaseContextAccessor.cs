using CT.Application.Interfaces;

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
