using CT.Application.Abstractions.Interfaces;
using CT.Application.Interfaces;
using CT.Application.Constants;

namespace CT.Gadgets.FunctionApp.Services;

public class UserContextAccessor(HttpRequestContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator) : BaseContextAccessor(httpContextAccessor, tokenGenerator), IUserContextAccessor
{
    public string? GetUserIdentifier()
    {
        return GetClaimValueFromBearerToken(CustomClaimTypes.UserIdentifier);
    }

    public string? GetSessionId()
    {
        return GetClaimValueFromBearerToken(CustomClaimTypes.SessionId);
    }
}
