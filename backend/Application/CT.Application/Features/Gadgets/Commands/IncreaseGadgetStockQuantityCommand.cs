using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using FluentValidation;
using MediatR;
using static CT.Application.Features.Gadgets.Commands.IncreaseGadgetStockQuantityCommand;

namespace CT.Application.Features.Gadgets.Commands;

public class IncreaseGadgetStockQuantityCommand(Guid gadgetId) : IRequest<BaseOutput<IncreaseGadgetStockQuantityResponseModel>>
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

    public class IncreaseGadgetStockQuantityCommandHandler : IRequestHandler<IncreaseGadgetStockQuantityCommand, BaseOutput<IncreaseGadgetStockQuantityResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository;

        public IncreaseGadgetStockQuantityCommandHandler(IGadgetsRepositoryService repository)
        {
            _repository = repository;
        }

        public async Task<BaseOutput<IncreaseGadgetStockQuantityResponseModel>> Handle(
            IncreaseGadgetStockQuantityCommand request,
            CancellationToken cancellationToken)
        {
            int stockQuantity = await _repository.IncreaseGadgetStockQuantityAsync(request.GadgetId, cancellationToken).ConfigureAwait(false);

            return new BaseOutput<IncreaseGadgetStockQuantityResponseModel>(
                Abstractions.Enums.OperationResult.Updated,
                new IncreaseGadgetStockQuantityResponseModel
                {
                    StockQuantity = stockQuantity
                });
        }
    }
}