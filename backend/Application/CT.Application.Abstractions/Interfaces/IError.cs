namespace CT.Application.Abstractions.Interfaces;

public interface IError
{
    string GetMessage();
    string GetUserFriendlyMessage();
}
