using FluentValidation;
using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using static CT.Application.Features.Gadgets.Commands.DeleteGadgetCommand;

namespace CT.Application.Features.Gadgets.Commands;

public class DeleteGadgetCommand(Guid GadgetId) : ContextualRequest, IRequest<BaseOutput<DeleteGadgetResponseModel>>
{
    public Guid GadgetId { get; set; } = GadgetId;

    public class DeleteGadgetResponseModel
    {
        public Guid GadgetId { get; set; }
    }

    public class DeleteGadgetCommandValidator : AbstractValidator<DeleteGadgetCommand>
    {
        public DeleteGadgetCommandValidator()
        {
            RuleFor(x => x.GadgetId).NotEmpty();
        }
    }

    public class DeleteGadgetCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<DeleteGadgetCommand, BaseOutput<DeleteGadgetResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<DeleteGadgetResponseModel>> Handle(DeleteGadgetCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteEntityHardAsync<Gadget>(request.GadgetId).ConfigureAwait(false);

            return new BaseOutput<DeleteGadgetResponseModel>(Abstractions.Enums.OperationResult.Deleted, new DeleteGadgetResponseModel() { GadgetId = request.GadgetId });
        }
    }
}
