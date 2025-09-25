using CT.Application.Abstractions.Interfaces;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Application.Models;
using FluentValidation;
using MediatR;
using static CT.Application.FeaturesIS.Login.Queries.BasicLoginUserQuery;
using static CT.Application.FeaturesIS.Login.Queries.RefreshLoginQuery;

namespace CT.Application.FeaturesIS.Login.Queries;

public class RefreshLoginQuery(RefreshLoginQueryRequestModel data) :  BaseInput<RefreshLoginQueryRequestModel>(data), IRequest<BaseOutput<RefreshLoginQueryResponseModel>>, IAuthenticatedRequest
{
    public class RefreshLoginQueryRequestModel(string refreshToken)
    {
        public string RefreshToken { get; set; } = refreshToken;
    }

    public class RefreshLoginQueryResponseModel : LoginUserResponse
    {
        public RefreshLoginQueryResponseModel() : base()
        {
        }
        public RefreshLoginQueryResponseModel(LoginUserResponse loginUserResponse) : this()
        {
            UserIdentifier = loginUserResponse.UserIdentifier;
            Result = loginUserResponse.Result;
            RefreshToken = loginUserResponse.RefreshToken;
            Token = loginUserResponse.Token;
            Message = loginUserResponse.Message;
        }
    }
    public class RefreshLoginCommandValidator : AbstractValidator<RefreshLoginQuery>
    {
        public RefreshLoginCommandValidator()
        {
            RuleFor(x => x.Model).NotEmpty().SetValidator(new RefreshLoginQueryModelValidator());
        }
    }

    public class RefreshLoginQueryModelValidator : AbstractValidator<RefreshLoginQueryRequestModel>
    {
        public RefreshLoginQueryModelValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}

public class RefreshLoginQueryHandler(IIdentityServerService identityServerService) : IRequestHandler<RefreshLoginQuery, BaseOutput<RefreshLoginQueryResponseModel>>
{
    private readonly IIdentityServerService _identityServerService = identityServerService;
    
    public async Task<BaseOutput<RefreshLoginQueryResponseModel>> Handle(RefreshLoginQuery request, CancellationToken cancellationToken)
    {
        var loginResponse = await _identityServerService.RefreshUserLoginAsync((string) request.Context[Constants.ContextKeys.UserIdentifier]!);
        var model = new RefreshLoginQueryResponseModel(loginResponse);

        if (!string.IsNullOrWhiteSpace(loginResponse!.Message))
        {
            return new BaseOutput<RefreshLoginQueryResponseModel>(Abstractions.Enums.OperationResult.Unauthorized, model);
        }

        return new BaseOutput<RefreshLoginQueryResponseModel>(Abstractions.Enums.OperationResult.Ok, model);
    }
}
