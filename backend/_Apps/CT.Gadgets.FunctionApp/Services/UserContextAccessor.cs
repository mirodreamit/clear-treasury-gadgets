using CT.Application.Abstractions.Interfaces;
using CT.Application.Interfaces;
using CT.Application.Constants;

namespace CT.Gadgets.FunctionApp.Services;

public class UserContextAccessor(HttpRequestContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator) : BaseContextAccessor(httpContextAccessor, tokenGenerator), IUserContextAccessor
{
    public string? GetUserIdentifier()
    {
        var userIdentifier = GetClaimValueFromBearerToken(CustomClaimTypes.UserIdentifier);

        if (string.IsNullOrWhiteSpace(userIdentifier))
        {
            userIdentifier ??= GetUserIdentifierFromCookie();
        }
        
        return userIdentifier;
    }

    public string? GetSessionId()
    {
        return GetClaimValueFromBearerToken(CustomClaimTypes.SessionId);
    }
}
