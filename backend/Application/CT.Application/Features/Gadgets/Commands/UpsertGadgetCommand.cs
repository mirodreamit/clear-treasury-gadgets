using FluentValidation;
using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using CT.Repository.Abstractions.Enums;
using static CT.Application.Features.Gadgets.Commands.UpsertGadgetCommand;
using CT.Application.Extensions;

namespace CT.Application.Features.Gadgets.Commands;

public class UpsertGadgetCommand(Guid gadgetId, CreateGadgetRequestModel data) : BaseInput<CreateGadgetRequestModel>(data), IRequest<BaseOutput<UpsertGadgetResponseModel>>
{
    public Guid GadgetId { get; set; } = gadgetId;

    public class CreateGadgetRequestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpsertGadgetResponseModel
    {
        public Guid Id { get; set; }
    }

    public class UpsertGadgetCommandValidator : AbstractValidator<UpsertGadgetCommand>
    {
        public UpsertGadgetCommandValidator()
        {
            RuleFor(x => x.GadgetId).NotEmpty();
            RuleFor(x => x.Model).NotEmpty().SetValidator(new UpsertGadgetCommandModelValidator());
        }
    }

    public class UpsertGadgetCommandModelValidator : AbstractValidator<CreateGadgetRequestModel>
    {
        public UpsertGadgetCommandModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
    
    //TODO: create PATCH method for increasing stock

    public class UpsertGadgetCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<UpsertGadgetCommand, BaseOutput<UpsertGadgetResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<UpsertGadgetResponseModel>> Handle(UpsertGadgetCommand request, CancellationToken cancellationToken)
        {
            var entity = new Gadget(request.GadgetId, request.Model.Name, request.Model.StockQuantity, request.Model.Description);

            var res = await _repository.UpsertEntityAsync(entity).ConfigureAwait(false);

            return new BaseOutput<UpsertGadgetResponseModel>(res.ToOperationResult(), new UpsertGadgetResponseModel()
            {
                Id = entity.Id
            });
        }
    }
}