using CT.Application.Abstractions.Enums;
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

    public class DecreaseGadgetStockQuantityCommandHandler(IGadgetsRepositoryService repository, IGadgetNotifier gadgetNotifier) : IRequestHandler<DecreaseGadgetStockQuantityCommand, BaseOutput<DecreaseGadgetStockQuantityResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;
        private readonly IGadgetNotifier _gadgetNotifier = gadgetNotifier;
        public async Task<BaseOutput<DecreaseGadgetStockQuantityResponseModel>> Handle(
            DecreaseGadgetStockQuantityCommand request,
            CancellationToken cancellationToken)
        {
            var userId = (Guid)request.Context[Constants.ContextKeys.UserId]!;
            var result = await _repository.DecreaseGadgetStockQuantityAsync(request.GadgetId, userId, cancellationToken).ConfigureAwait(false);
            
            var responseModel = new DecreaseGadgetStockQuantityResponseModel
            {
                StockQuantity = result.StockQuantity
            };

            if (!result.Success)
            {
                return new BaseOutput<DecreaseGadgetStockQuantityResponseModel>(
                    OperationResult.BadRequest,
                    result.ErrorMessage,
                    responseModel);
            }

            await _gadgetNotifier.NotifyStockChangeAsync(request.GadgetId.ToString(), result.StockQuantity).ConfigureAwait(false);

            return new BaseOutput<DecreaseGadgetStockQuantityResponseModel>(
                OperationResult.Updated,
                responseModel);
        }
    }
}
