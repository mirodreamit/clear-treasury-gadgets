using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using CT.Repository.Abstractions.Enums;
using FluentValidation;
using MediatR;
using CT.Application.Extensions;
using static CT.Application.Features.Categories.Commands.UpsertCategoryCommand;

namespace CT.Application.Features.Categories.Commands;

public class UpsertCategoryCommand(Guid id, CreateCategoryRequestModel data) : BaseInput<CreateCategoryRequestModel>(data), IRequest<BaseOutput<UpsertCategoryResponseModel>>
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
            var entity = new Category(request.Id, request.Model.Name);

            var res = await _repository.UpsertAsync(entity).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return new BaseOutput<UpsertCategoryResponseModel>(res.ToOperationResult(), new UpsertCategoryResponseModel()
            {
                CategoryId = entity.Id
            });
        }
    }
}