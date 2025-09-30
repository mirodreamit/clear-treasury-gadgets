using CT.Application.Abstractions.Interfaces;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using static CT.Application.Features.Gadgets.Commands.DeleteGadgetFullBatchCommand;

namespace CT.Application.Features.Gadgets.Commands;

public class DeleteGadgetFullBatchCommand(DeleteGadgetFullBatchRequestModel model) : BaseInput<DeleteGadgetFullBatchRequestModel>(model), IRequest<BaseOutput<DeleteGadgetFullBatchResponseModel>>, IAuthenticatedRequest
{
    public class DeleteGadgetFullBatchRequestModel
    {
        public List<Guid> GadgetIds { get; set; }
    }


    public class DeleteGadgetFullBatchResponseModel
    {
        public List<Guid> GadgetIds { get; set; }
    }

    public class DeleteGadgetFullBatchCommandValidator : AbstractValidator<DeleteGadgetFullBatchCommand>
    {
        public DeleteGadgetFullBatchCommandValidator()
        {
            RuleFor(x => x.Model).NotEmpty().SetValidator(new DeleteGadgetFullBatchRequestModelValidator());
        }
    }
    public class DeleteGadgetFullBatchRequestModelValidator : AbstractValidator<DeleteGadgetFullBatchRequestModel>
    {
        public DeleteGadgetFullBatchRequestModelValidator()
        {
            RuleFor(x => x.GadgetIds).NotEmpty();
        }
    }

    public class DeleteGadgetFullBatchCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<DeleteGadgetFullBatchCommand, BaseOutput<DeleteGadgetFullBatchResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<DeleteGadgetFullBatchResponseModel>> Handle(DeleteGadgetFullBatchCommand request, CancellationToken cancellationToken)
        {
            var transactionModel = await _repository.BeginTransactionAsync().ConfigureAwait(false);
            var transaction = (IDbContextTransaction)transactionModel.Transaction!;

            try
            {
                var gadgetCategoryIds = await _repository.QueryAsync(GetGadgetCategoryIdsQuery(request.Model.GadgetIds)).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                if (gadgetCategoryIds.Records?.Count > 0)
                {
                    await _repository.DeleteHardRangeAsync<GadgetCategory>(gadgetCategoryIds.Records.Select(x => x.Id).ToList());
                }
                
                cancellationToken.ThrowIfCancellationRequested();

                await _repository.DeleteHardRangeAsync<Gadget>(request.Model.GadgetIds);

                cancellationToken.ThrowIfCancellationRequested();

                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);

                throw;
            }

            return new BaseOutput<DeleteGadgetFullBatchResponseModel>(Abstractions.Enums.OperationResult.Deleted, new DeleteGadgetFullBatchResponseModel() { GadgetIds = request.Model.GadgetIds });
        }
        private class GadgetCategoryId
        { 
            public Guid Id { get; set; }
        }

        private IQueryable<GadgetCategoryId> GetGadgetCategoryIdsQuery(List<Guid> gadgetIds)
        {
            var ctx = _repository.DbContext;

            var query =
                from gc in ctx.GadgetCategory
                where gadgetIds.Contains(gc.GadgetId)
                select new GadgetCategoryId()
                {
                    Id = gc.Id
                };
                
            return query;
        }
    }
}
