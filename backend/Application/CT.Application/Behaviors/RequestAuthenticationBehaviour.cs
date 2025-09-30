using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Interfaces;
using CT.Application.Interfaces;
using CT.Application.Models;
using CT.Domain.Entities;
using MediatR;
using System.Net;

namespace CT.Application.Behaviors;

public class RequestAuthenticationBehaviour<TRequest, TResponse>(IUserContextAccessor userContextAccessor, IGadgetsRepositoryService repository) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IAuthenticatedRequest
{
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IGadgetsRepositoryService _repository = repository;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string? errorMessage = null;

        try
        {
            var userIdentifier = _userContextAccessor.GetUserIdentifier();

            if (userIdentifier == null)
            {
                errorMessage = "UserIdentifier not found. Please check provided Bearer token.";
            }
            else
            {
                var userId = await _repository.GetIdAsync<User>(x => x.Identifier == userIdentifier).ConfigureAwait(false);

                if (userId is null)
                {
                    errorMessage = $"User for the given key not found. [UserIdentifier: {userIdentifier}]";
                }
                else
                {
                    if (request is IContextualRequest ctxRequest)
                    {
                        ctxRequest.Context.Add(Constants.ContextKeys.UserId, userId);
                        ctxRequest.Context.Add(Constants.ContextKeys.UserIdentifier, userIdentifier);
                    }
                }
            }
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException ex)
        {
            errorMessage = ex.Message;
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException)
        {
            errorMessage = "Token signature not valid. Plase check provided Bearer token.";
        }
        catch (Exception)
        {
            errorMessage = "Error reading user identifier. Plase check provided Bearer token.";
        }

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            var ctor = typeof(TResponse).GetConstructor([
                typeof(OperationResult),
                typeof(string),
                typeof(ApplicationError)
            ]);

            if (ctor != null)
            {
                return (TResponse)ctor.Invoke(
                [
                    OperationResult.Unauthorized,
                    HttpStatusCode.Unauthorized.ToString(),
                    new ApplicationError(new {
                        TokenExpired = true,
                        ErrorMessage = errorMessage
                    })
                ]);
            }

            throw new InvalidOperationException(
                $"Cannot construct {typeof(TResponse).Name}. " +
                "Expected a constructor with (OperationResult, int, ApplicationError).");
        }

        return await next();
    }
}
