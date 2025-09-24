using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CT.FunctionApi.Services;

public abstract class BaseContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public BaseContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

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
            if (requestHeaders is not null && requestHeaders.ContainsKey(headerKey))
            {
                var headers = requestHeaders[headerKey];
                foreach (var headerValue in headers)
                {
                    headerValues.Add(headerValue);
                }
            }
        }

        return headerValues;
    }
}
