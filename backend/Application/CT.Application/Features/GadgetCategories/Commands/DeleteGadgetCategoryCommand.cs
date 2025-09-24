using FluentValidation;
using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using static CT.Application.Features.GadgetCategories.Commands.DeleteGadgetCategoryCommand;
using CT.Application.Abstractions.Enums;

namespace CT.Application.Features.GadgetCategories.Commands;

public class DeleteGadgetCategoryCommand(Guid gadgetCategoryId) : ContextualRequest, IRequest<BaseOutput<DeleteGadgetCategoryResponseModel>>
{
    public Guid GadgetCategoryId { get; set; } = gadgetCategoryId;

    public class DeleteGadgetCategoryResponseModel
    {
        public Guid GadgetCategoryId { get; set; }
    }

    public class DeleteGadgetCategoryCommandValidator : AbstractValidator<DeleteGadgetCategoryCommand>
    {
        public DeleteGadgetCategoryCommandValidator()
        {
            RuleFor(x => x.GadgetCategoryId).NotEmpty();
        }
    }

    public class DeleteGadgetCategoryCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<DeleteGadgetCategoryCommand, BaseOutput<DeleteGadgetCategoryResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<DeleteGadgetCategoryResponseModel>> Handle(DeleteGadgetCategoryCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteEntityHardAsync<GadgetCategory>(request.GadgetCategoryId).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return new BaseOutput<DeleteGadgetCategoryResponseModel>(OperationResult.Deleted, new DeleteGadgetCategoryResponseModel() { GadgetCategoryId = request.GadgetCategoryId });
        }
    }
}
