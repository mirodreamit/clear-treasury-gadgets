using CT.Application.Abstractions.Interfaces;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using FluentValidation;
using MediatR;
using static CT.Application.Features.Gadgets.Commands.DecreaseGadgetStockQuantityCommand;

namespace CT.Application.Features.Gadgets.Commands;

public class DecreaseGadgetStockQuantityCommand(Guid gadgetId) : ContextualRequest, IRequest<BaseOutput<DecreaseGadgetStockQuantityResponseModel>>, IAuthenticatedRequest
{
    public Guid GadgetId { get; set; } = gadgetId;

    public class DecreaseGadgetStockQuantityResponseModel
    {
        public int StockQuantity { get; set; }
    }

    public class DecreaseGadgetStockQuantityCommandValidator : AbstractValidator<DecreaseGadgetStockQuantityCommand>
    {
        public DecreaseGadgetStockQuantityCommandValidator()
        {
            RuleFor(x => x.GadgetId).NotEmpty();
        }
    }

    public class DecreaseGadgetStockQuantityCommandHandler : IRequestHandler<DecreaseGadgetStockQuantityCommand, BaseOutput<DecreaseGadgetStockQuantityResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository;


        public DecreaseGadgetStockQuantityCommandHandler(IGadgetsRepositoryService repository)
        {
            _repository = repository;
        }

        public async Task<BaseOutput<DecreaseGadgetStockQuantityResponseModel>> Handle(
            DecreaseGadgetStockQuantityCommand request,
            CancellationToken cancellationToken)
        {
            var userId = (Guid)request.Context[Constants.ContextKeys.UserId]!;
            int stockQuantity = await _repository.DecreaseGadgetStockQuantityAsync(request.GadgetId, userId, cancellationToken).ConfigureAwait(false);

            return new BaseOutput<DecreaseGadgetStockQuantityResponseModel>(
                Abstractions.Enums.OperationResult.Updated,
                new DecreaseGadgetStockQuantityResponseModel
                {
                    StockQuantity = stockQuantity
                });
        }
    }
}