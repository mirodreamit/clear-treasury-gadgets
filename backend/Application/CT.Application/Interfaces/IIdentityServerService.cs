using CT.Application.Models;

namespace CT.Application.Interfaces;

public interface IIdentityServerService
{
    Task<RegisterUserResponse> BasicRegisterUserAsync(string email, string password, string displayName);
    Task<LoginUserResponse> BasicUserLoginAsync(string email, string password);

    Task<AnonymousUserLoginResponse> AnonymousRegisterUserAsync();
    Task<LoginUserResponse> AnonymousUserLoginAsync(string sessionId);
    
    Task<LoginUserResponse> RefreshUserLoginAsync(string userIdentifier);
    Task<LoginUserResponse> RefreshAnonymousUserLoginAsync(string userIdentifier);
}
