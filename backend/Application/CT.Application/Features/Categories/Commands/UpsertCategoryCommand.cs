using CT.Application.Abstractions.Interfaces;
using CT.Application.Abstractions.Models;
using CT.Application.Extensions;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using FluentValidation;
using MediatR;
using static CT.Application.Features.Categories.Commands.UpsertCategoryCommand;
using static CT.Application.Features.Gadgets.Commands.UpsertGadgetCommand;

namespace CT.Application.Features.Categories.Commands;

public class UpsertCategoryCommand(Guid id, CreateCategoryRequestModel data) : BaseInput<CreateCategoryRequestModel>(data), IRequest<BaseOutput<UpsertCategoryResponseModel>>, IAuthenticatedRequest
{
    public Guid Id { get; set; } = id;

    public class CreateCategoryRequestModel
    {
        public string Name { get; set; }
    }

    public class UpsertCategoryResponseModel
    {
        public Guid CategoryId { get; set; }
    }

    public class UpsertCategoryCommandValidator : AbstractValidator<UpsertCategoryCommand>
    {
        public UpsertCategoryCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Model).NotEmpty().SetValidator(new UpsertCategoryCommandModelValidator());
        }
    }

    public class UpsertCategoryCommandModelValidator : AbstractValidator<CreateCategoryRequestModel>
    {
        public UpsertCategoryCommandModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class UpsertCategoryCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<UpsertCategoryCommand, BaseOutput<UpsertCategoryResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<UpsertCategoryResponseModel>> Handle(UpsertCategoryCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetIdAsync<Category>(x => x.Name.ToLower() == request.Model.Name.ToLower()).ConfigureAwait(false);

            if (existing != null)
            {
                return new BaseOutput<UpsertCategoryResponseModel>(Abstractions.Enums.OperationResult.Conflict, $"Entity with the given key already exists. [Name = '{request.Model.Name}']", null!);
            }

            var entity = new Category(request.Id, request.Model.Name, (Guid)request.Context[Constants.ContextKeys.UserId]!);

            var res = await _repository.UpsertAsync(entity).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return new BaseOutput<UpsertCategoryResponseModel>(res.ToOperationResult(), new UpsertCategoryResponseModel()
            {
                CategoryId = entity.Id
            });
        }
    }
}