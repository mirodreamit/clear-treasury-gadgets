using CT.Application.Abstractions.Interfaces;

namespace CT.Tests.Services;

public class UserContextAccessor: IUserContextAccessor
{
    public string GetUserIdentifier()
    {
        return "auth|12345";
        //return null!;
    }

    public string GetSessionId()
    {
        return "s|12345";
    }
}
