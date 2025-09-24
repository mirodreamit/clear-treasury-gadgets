using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using FluentValidation;
using MediatR;
using static CT.Application.Features.Gadgets.Commands.DecreaseGadgetStockQuantityCommand;

namespace CT.Application.Features.Gadgets.Commands;

public class DecreaseGadgetStockQuantityCommand(Guid gadgetId) : IRequest<BaseOutput<DecreaseGadgetStockQuantityResponseModel>>
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
            int stockQuantity = await _repository.DecreaseGadgetStockQuantityAsync(request.GadgetId, cancellationToken).ConfigureAwait(false);

            return new BaseOutput<DecreaseGadgetStockQuantityResponseModel>(
                Abstractions.Enums.OperationResult.Updated,
                new DecreaseGadgetStockQuantityResponseModel
                {
                    StockQuantity = stockQuantity
                });
        }
    }
}