using FluentValidation;
using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using CT.Repository.Abstractions.Enums;
using static CT.Application.Features.GadgetCategories.Commands.UpsertGadgetCategoryCommand;
using CT.Application.Extensions;
using CT.Application.Abstractions.Interfaces;

namespace CT.Application.Features.GadgetCategories.Commands;

public class UpsertGadgetCategoryCommand(Guid gadgetCategoryId, CreateGadgetCategoryRequestModel data) : BaseInput<CreateGadgetCategoryRequestModel>(data), IRequest<BaseOutput<UpsertGadgetCategoryResponseModel>>, IAuthenticatedRequest
{
    public Guid GadgetCategoryId { get; set; } = gadgetCategoryId;

    public class CreateGadgetCategoryRequestModel
    {
        public Guid GadgetId { get; set; }
        public Guid CategoryId{ get; set; }
        public int Ordinal { get; set; }
    }

    public class UpsertGadgetCategoryResponseModel
    {
        public Guid Id { get; set; }
    }

    public class UpsertGadgetCategoryCommandValidator : AbstractValidator<UpsertGadgetCategoryCommand>
    {
        public UpsertGadgetCategoryCommandValidator()
        {
            RuleFor(x => x.GadgetCategoryId).NotEmpty();
            RuleFor(x => x.Model).NotEmpty().SetValidator(new UpsertGadgetCategoryCommandModelValidator());
        }
    }

    public class UpsertGadgetCategoryCommandModelValidator : AbstractValidator<CreateGadgetCategoryRequestModel>
    {
        public UpsertGadgetCategoryCommandModelValidator()
        {
            RuleFor(x => x.GadgetId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Ordinal).GreaterThanOrEqualTo(0);
        }
    }

    public class UpsertGadgetCategoryCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<UpsertGadgetCategoryCommand, BaseOutput<UpsertGadgetCategoryResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<UpsertGadgetCategoryResponseModel>> Handle(UpsertGadgetCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = new GadgetCategory(request.GadgetCategoryId, request.Model.GadgetId, request.Model.CategoryId, request.Model.Ordinal, (Guid)request.Context[Constants.ContextKeys.UserId]!);

            var res = await _repository.UpsertAsync(entity).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return new BaseOutput<UpsertGadgetCategoryResponseModel>(res.ToOperationResult(), new UpsertGadgetCategoryResponseModel()
            {
                Id = entity.Id
            });
        }
    }
}