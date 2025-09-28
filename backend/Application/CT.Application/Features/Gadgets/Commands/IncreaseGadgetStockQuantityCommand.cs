using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Interfaces;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using FluentValidation;
using MediatR;
using static CT.Application.Features.Gadgets.Commands.DecreaseGadgetStockQuantityCommand;
using static CT.Application.Features.Gadgets.Commands.IncreaseGadgetStockQuantityCommand;

namespace CT.Application.Features.Gadgets.Commands;

public class IncreaseGadgetStockQuantityCommand(Guid gadgetId) : ContextualRequest, IRequest<BaseOutput<IncreaseGadgetStockQuantityResponseModel>>, IAuthenticatedRequest
{
    public Guid GadgetId { get; set; } = gadgetId;

    public class IncreaseGadgetStockQuantityResponseModel
    {
        public int StockQuantity { get; set; }
    }

    public class IncreaseGadgetStockQuantityCommandValidator : AbstractValidator<IncreaseGadgetStockQuantityCommand>
    {
        public IncreaseGadgetStockQuantityCommandValidator()
        {
            RuleFor(x => x.GadgetId).NotEmpty();
        }
    }

    public class IncreaseGadgetStockQuantityCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<IncreaseGadgetStockQuantityCommand, BaseOutput<IncreaseGadgetStockQuantityResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<IncreaseGadgetStockQuantityResponseModel>> Handle(
            IncreaseGadgetStockQuantityCommand request,
            CancellationToken cancellationToken)
        {
            var userId = (Guid)request.Context[Constants.ContextKeys.UserId]!;
            var result = await _repository.IncreaseGadgetStockQuantityAsync(request.GadgetId, userId, cancellationToken).ConfigureAwait(false);

            var responseModel = new IncreaseGadgetStockQuantityResponseModel
            {
                StockQuantity = result.StockQuantity
            };

            if (!result.Success)
            {
                return new BaseOutput<IncreaseGadgetStockQuantityResponseModel>(
                    OperationResult.BadRequest,
                    responseModel);
            }

            return new BaseOutput<IncreaseGadgetStockQuantityResponseModel>(
                OperationResult.Updated,
                responseModel);
        }
    }
}