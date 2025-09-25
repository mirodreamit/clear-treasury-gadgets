using CT.Application.Abstractions.Enums;
using CT.Application.Interfaces;
using CT.Application.Models;
using CT.Domain.IdentityServer;
using CT.Repository.Abstractions.Models;
using System.Security.Claims;

namespace CT.Application.Services;

internal class IdentityServerService(IIdentityServerRepositoryService repository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher) : IIdentityServerService
{
    private readonly IIdentityServerRepositoryService _repository = repository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<RegisterUserResponse> BasicRegisterUserAsync(string email, string password, string displayName)
    {
        var existingUserId = await _repository.GetIdAsync<UserDetail>(x => x.Email.ToLower() == email.ToLower()).ConfigureAwait(false);

        if (existingUserId is not null)
        {
            return new RegisterUserResponse()
            {
                Message = $"User with the given email has already registered. [Email: {email}]",
                Result = RegisterUserResult.AlreadyRegistered
            };
        }

        var userId = Guid.NewGuid();

        var userIdentifier = GenerateUserIdentifier(userId);

        var user = new User
        {
            Id = userId,
            Identifier = userIdentifier,
            IsSuperAdmin = false,
            IsBlocked = false
        };

        var userDetail = new UserDetail
        {
            Id = userId,
            Email = email,
            DisplayName = displayName
        };

        var userCredential = new UserCredential
        {
            Id = userId,
            PasswordHash = _passwordHasher.HashPassword(password)
        };

        TransactionModel transactionModel = null!;

        try
        {
            transactionModel = await _repository.BeginTransactionAsync().ConfigureAwait(false);

            await _repository.UpsertAsync(user).ConfigureAwait(false);
            await _repository.UpsertAsync(userDetail).ConfigureAwait(false);
            await _repository.UpsertAsync(userCredential).ConfigureAwait(false);

            await _repository.CommitTransactionAsync(transactionModel).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await _repository.RollbackTransactionAsync(transactionModel).ConfigureAwait(false);
            throw;
        }

        var loginResponse = await BasicUserLoginAsync(email, password);

        var model = new RegisterUserResponse
        {
            UserIdentifier = loginResponse?.UserIdentifier,
            Token = loginResponse?.Token,
            RefreshToken = loginResponse?.RefreshToken,
            Message = loginResponse?.Message,
            Result = RegisterUserResult.Success
        };

        return model;
    }
    public async Task<LoginUserResponse> BasicUserLoginAsync(string email, string password)
    {
        var userDetail = await _repository.GetSingleAsync<UserDetail>(x => email.ToLower() == x.Email.ToLower()).ConfigureAwait(false);

        if (userDetail is null)
        {
            return new LoginUserResponse
            {
                Message = "User with the given email not found",
                Result = LoginUserResult.UserNotFound
            };
        }

        var userCredential = await _repository.GetByIdAsync<UserCredential>(userDetail!.Id).ConfigureAwait(false);

        var user = await _repository.GetByIdAsync<User>(userDetail!.Id).ConfigureAwait(false);

        var passwordVerified = _passwordHasher.VerifyPassword(password, userCredential!.PasswordHash);

        if (!passwordVerified)
        {
            return new LoginUserResponse
            {
                Message = "Wrong password",
                Result = LoginUserResult.WrongPassword
            };
        }
        
        var loginResponseModel = new LoginUserResponse()
        {
            UserIdentifier = user!.Identifier!,
            Token = CreateUserToken(user, userDetail),
            RefreshToken = CreateUserRefreshToken(user),
            Result = LoginUserResult.Success
        };

        return loginResponseModel;
    }

    public async Task<LoginUserResponse> RefreshUserLoginAsync(string userIdentifier)
    {
        var user = await _repository.GetSingleAsync<User>(x => x.Identifier == userIdentifier).ConfigureAwait(false);

        if (user is null)
        {
            return new LoginUserResponse
            {
                Message = "User with the given identifier not found",
                Result = LoginUserResult.UserNotFound
            };
        }

        var userDetail = await _repository.GetByIdAsync<UserDetail>(user.Id).ConfigureAwait(false);

        if (userDetail is null)
        {
            return new LoginUserResponse()
            {
                Message = "User with the given identifier not found",
                Result = LoginUserResult.UserNotFound
            };
        }

        return new LoginUserResponse()
        {
            UserIdentifier = user!.Identifier!,
            Token = CreateUserToken(user, userDetail),
            RefreshToken = CreateUserRefreshToken(user),
            Result = LoginUserResult.Success
        };
    }

    public Task<AnonymousUserLoginResponse> AnonymousRegisterUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task<LoginUserResponse> AnonymousUserLoginAsync(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<LoginUserResponse> RefreshAnonymousUserLoginAsync(string userIdentifier)
    {
        throw new NotImplementedException();
    }

    #region private_methods

    private static Claim GetClaim(string name, string value)
    {
        return new Claim(name, value, ClaimValueTypes.String, "CT API", "CT API");
    }

    private string CreateUserToken(User principal, UserDetail userDetail)
    {
        var claims = new List<Claim>
        {
            GetClaim("email", userDetail.Email),
            GetClaim("displayName", userDetail.DisplayName),
            GetClaim("userIdentifier", principal.Identifier)
        };

        return _tokenGenerator.CreateAccessToken(claims);
    }

    private string CreateUserRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            GetClaim("userIdentifier", user.Identifier)
        };

        return _tokenGenerator.CreateRefreshToken(claims);
    }

    private async Task<string> CreateAnonymousUserToken(User user, AnonymousUser anonymousUser, Func<string, List<Claim>, Task>? onAnonymousUserTokenCreatingAsync)
    {
        var claims = new List<Claim>
        {
            GetClaim("email", string.Empty),
            GetClaim("displayName", "Guest"),
            GetClaim("userIdentifier", user.Identifier),
            GetClaim("sessionId", anonymousUser.SessionId)
        };

        if (onAnonymousUserTokenCreatingAsync != null)
        {
            await onAnonymousUserTokenCreatingAsync.Invoke(user.Identifier, claims);
        }

        return _tokenGenerator.CreateAccessToken(claims);
    }

    private string CreateAnonymousUserRefreshToken(User user, AnonymousUser anonymousUser)
    {
        var claims = new List<Claim>
        {
            GetClaim("userIdentifier", user.Identifier),
            GetClaim("sessionId", anonymousUser.SessionId)
        };

        return _tokenGenerator.CreateRefreshToken(claims);
    }
    private static string GenerateUserIdentifier(Guid userId)
    {
        return $"auth|{userId:N}";
    }

    #endregion
}
