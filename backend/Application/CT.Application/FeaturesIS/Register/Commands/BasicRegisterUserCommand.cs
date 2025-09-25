using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Application.Models;
using FluentValidation;
using MediatR;
using static CT.Application.FeaturesIS.Register.Commands.BasicRegisterUserCommand;

namespace CT.Application.FeaturesIS.Register.Commands;

public class BasicRegisterUserCommand(BasicRegisterUserCommandRequestModel data) : BaseInput<BasicRegisterUserCommandRequestModel>(data), IRequest<BaseOutput<BasicRegisterUserCommandResponseModel>>
{
    public class BasicRegisterUserCommandRequestModel(string email, string password)
    {
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;
        public string DisplayName { get; set; } = password;
    }

    public class BasicRegisterUserCommandResponseModel : RegisterUserResponse
    {
        public BasicRegisterUserCommandResponseModel() : base()
        {
        }
        public BasicRegisterUserCommandResponseModel(RegisterUserResponse registerUserResponse) : this()
        {
            UserIdentifier = registerUserResponse.UserIdentifier;
            Result = registerUserResponse.Result;
            RefreshToken = registerUserResponse.RefreshToken;
            Token = registerUserResponse.Token;
            Message = registerUserResponse.Message;
        }
    }
    public class BasicRegisterUserCommandValidator : AbstractValidator<BasicRegisterUserCommand>
    {
        public BasicRegisterUserCommandValidator()
        {
            RuleFor(x => x.Model).NotEmpty().SetValidator(new BasicRegisterUserCommandModelValidator());
        }
    }

    public class BasicRegisterUserCommandModelValidator : AbstractValidator<BasicRegisterUserCommandRequestModel>
    {
        public BasicRegisterUserCommandModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}

public class BasicRegisterUserCommandHandler(IIdentityServerService identityServerService) : IRequestHandler<BasicRegisterUserCommand, BaseOutput<BasicRegisterUserCommandResponseModel>>
{
    private readonly IIdentityServerService _identityServerService = identityServerService;

    public async Task<BaseOutput<BasicRegisterUserCommandResponseModel>> Handle(BasicRegisterUserCommand request, CancellationToken cancellationToken)
    {
        var registerUserResponse = await _identityServerService.BasicRegisterUserAsync(request.Model.Email, request.Model.Password, request.Model.DisplayName).ConfigureAwait(false);
        var model = new BasicRegisterUserCommandResponseModel(registerUserResponse);

        if (!string.IsNullOrWhiteSpace(registerUserResponse.Message))
        {
            
            return new BaseOutput<BasicRegisterUserCommandResponseModel>(Abstractions.Enums.OperationResult.InternalError, model)
            { 
                Message = model.Message
            };
        }

        return new BaseOutput<BasicRegisterUserCommandResponseModel>(Abstractions.Enums.OperationResult.Created, model);
    }
}
