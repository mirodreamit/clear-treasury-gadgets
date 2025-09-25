using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using CT.Application.Interfaces;
namespace CT.FunctionApp.Services;

public abstract class BaseContextAccessor(IHttpContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;

    protected string GetHeaderValue(string header)
    {
        return GetHeaderValues(header)?.FirstOrDefault();
    }

    protected List<string> GetHeaderValues(string headerKey)
    {
        var headerValues = new List<string>();

        if (_httpContextAccessor.HttpContext is not null)
        {
            IHeaderDictionary requestHeaders = _httpContextAccessor.HttpContext.Request.Headers;
            
            if (requestHeaders is not null && requestHeaders.TryGetValue(headerKey, out Microsoft.Extensions.Primitives.StringValues headers))
            {
                foreach (var headerValue in headers)
                {
                    headerValues.Add(headerValue);
                }
            }
        }

        return headerValues;
    }

    protected JwtSecurityToken GetValidTokenFromHeader()
    {
        if (_tokenGenerator == null)
        {
            throw new ArgumentException("TokenGenerator cannot be null.");
        }

        JwtSecurityToken token = null;

        var authorizationHeaders = GetHeaderValues("authorization");

        if (authorizationHeaders.Count > 0)
        {
            var bearerToken = authorizationHeaders.First()["Bearer ".Length..];

            if (_tokenGenerator.ValidateToken(bearerToken, out _))
            {
                token = _tokenGenerator.DecodeToken(bearerToken);
            }
        }

        return token;
    }

    protected string GetClaimValue(string claimType)
    {
        var token = GetValidTokenFromHeader();

        if (token is not null)
        {
            var claim = token.Claims.FirstOrDefault(c => c.Type == claimType);

            if (claim is not null)
            {
                return claim.Value;
            }
        }

        return null!;
    }
}
