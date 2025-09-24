namespace CT.Application.Abstractions.Interfaces;

public interface IUserContextAccessor
{
    string GetUserIdentifier();
    string GetSessionId();
}
