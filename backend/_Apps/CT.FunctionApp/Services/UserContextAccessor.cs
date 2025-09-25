using Microsoft.AspNetCore.Http;
using CT.Application.Abstractions.Interfaces;
using CT.Application.Interfaces;
using CT.Application.Constants;

namespace CT.FunctionApp.Services;

public class UserContextAccessor(IHttpContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator) : BaseContextAccessor(httpContextAccessor, tokenGenerator), IUserContextAccessor
{
    public string GetUserIdentifier()
    {
        return GetClaimValue(CustomClaimTypes.UserIdentifier);
    }

    public string GetSessionId()
    {
        return GetClaimValue(CustomClaimTypes.SessionId);
    }
}
