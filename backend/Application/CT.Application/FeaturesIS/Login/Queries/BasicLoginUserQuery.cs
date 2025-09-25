using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Application.Models;
using FluentValidation;
using MediatR;
using static CT.Application.FeaturesIS.Login.Queries.BasicLoginUserQuery;

namespace CT.Application.FeaturesIS.Login.Queries;

public class BasicLoginUserQuery(BasicLoginUserQueryRequestModel data) : BaseInput<BasicLoginUserQueryRequestModel>(data), IRequest<BaseOutput<BasicLoginUserQueryResponseModel>>
{
    public class BasicLoginUserQueryRequestModel(string email, string password)
    {
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;
    }

    public class BasicLoginUserQueryResponseModel : LoginUserResponse
    {
        public BasicLoginUserQueryResponseModel() : base()
        {
        }
        public BasicLoginUserQueryResponseModel(LoginUserResponse loginUserResponse) : this()
        {
            UserIdentifier = loginUserResponse.UserIdentifier;
            Result = loginUserResponse.Result;
            RefreshToken = loginUserResponse.RefreshToken;
            Token = loginUserResponse.Token;
            Message = loginUserResponse.Message;
        }
    }
    public class BasicLoginUserCommandValidator : AbstractValidator<BasicLoginUserQuery>
    {
        public BasicLoginUserCommandValidator()
        {
            RuleFor(x => x.Model).NotEmpty().SetValidator(new BasicLoginUserQueryModelValidator());
        }
    }

    public class BasicLoginUserQueryModelValidator : AbstractValidator<BasicLoginUserQueryRequestModel>
    {
        public BasicLoginUserQueryModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}

public class BasicLoginUserQueryHandler(IIdentityServerService identityServerService) : IRequestHandler<BasicLoginUserQuery, BaseOutput<BasicLoginUserQueryResponseModel>>
{
    private readonly IIdentityServerService _identityServerService = identityServerService;

    public async Task<BaseOutput<BasicLoginUserQueryResponseModel>> Handle(BasicLoginUserQuery request, CancellationToken cancellationToken)
    {
        var loginResponse = await _identityServerService.BasicUserLoginAsync(request.Model.Email, request.Model.Password);
        var model = new BasicLoginUserQueryResponseModel(loginResponse);

        if (!string.IsNullOrWhiteSpace(loginResponse!.Message))
        {
            return new BaseOutput<BasicLoginUserQueryResponseModel>(Abstractions.Enums.OperationResult.Unauthorized, model);
        }

        return new BaseOutput<BasicLoginUserQueryResponseModel>(Abstractions.Enums.OperationResult.Ok, model);
    }
}
