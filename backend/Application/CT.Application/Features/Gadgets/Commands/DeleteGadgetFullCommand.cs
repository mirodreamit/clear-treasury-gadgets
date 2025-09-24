using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using static CT.Application.Features.Gadgets.Commands.DeleteGadgetFullCommand;

namespace CT.Application.Features.Gadgets.Commands;

public class DeleteGadgetFullCommand(Guid gadgetId) : ContextualRequest, IRequest<BaseOutput<DeleteGadgetFullResponseModel>>
{
    public Guid GadgetId { get; set; } = gadgetId;

    public class DeleteGadgetFullResponseModel
    {
        public Guid GadgetId { get; set; }
    }

    public class DeleteGadgetFullCommandValidator : AbstractValidator<DeleteGadgetFullCommand>
    {
        public DeleteGadgetFullCommandValidator()
        {
            RuleFor(x => x.GadgetId).NotEmpty();
        }
    }

    public class DeleteGadgetFullCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<DeleteGadgetFullCommand, BaseOutput<DeleteGadgetFullResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<DeleteGadgetFullResponseModel>> Handle(DeleteGadgetFullCommand request, CancellationToken cancellationToken)
        {
            var transactionModel = await _repository.BeginTransactionAsync().ConfigureAwait(false);
            var transaction = (IDbContextTransaction)transactionModel.Transaction!;

            try
            {
                await _repository.DeleteEntitiesHardByExpressionAsync<GadgetCategory>(x => x.GadgetId == request.GadgetId).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                await _repository.DeleteEntityHardAsync<Gadget>(request.GadgetId).ConfigureAwait(false);
                
                cancellationToken.ThrowIfCancellationRequested();

                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);

                throw;
            }

            return new BaseOutput<DeleteGadgetFullResponseModel>(Abstractions.Enums.OperationResult.Deleted, new DeleteGadgetFullResponseModel() { GadgetId = request.GadgetId });
        }
    }
}
