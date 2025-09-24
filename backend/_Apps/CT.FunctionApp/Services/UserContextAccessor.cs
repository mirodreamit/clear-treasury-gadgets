using Microsoft.AspNetCore.Http;
using CT.Application.Abstractions.Interfaces;

namespace CT.FunctionApi.Services;

public class UserContextAccessor : BaseContextAccessor, IUserContextAccessor
{
    public UserContextAccessor(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
    }

    public string GetUserIdentifier()
    {
        return null;
    }

    public string GetSessionId()
    {
        return null;
    }
}
