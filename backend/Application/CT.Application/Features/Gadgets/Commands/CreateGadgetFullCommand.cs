using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using static CT.Application.Features.Gadgets.Commands.CreateGadgetFullCommand;
using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Interfaces;

namespace CT.Application.Features.Gadgets.Commands;

public class CreateGadgetFullCommand(Guid gadgetId, CreateGadgetFullRequestModel data) : BaseInput<CreateGadgetFullRequestModel>(data), IRequest<BaseOutput<CreateGadgetFullResponseModel>>, IAuthenticatedRequest
{
    public Guid GadgetId { get; set; } = gadgetId;

    public class CreateGadgetFullRequestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public List<CreateAssignCategoryRequestModel> Categories { get; set; }
        
    }
    public class CreateAssignCategoryRequestModel
    {
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class CreateGadgetFullResponseModel
    {
        public Guid Id { get; set; }
        public List<Guid> CategoriesCreatedIds { get; set; } = [];
    }

    public class CreateGadgetFullCommandValidator : AbstractValidator<CreateGadgetFullCommand>
    {
        public CreateGadgetFullCommandValidator()
        {
            RuleFor(x => x.GadgetId).NotEmpty();
            RuleFor(x => x.Model).NotEmpty().SetValidator(new CreateGadgetFullCommandModelValidator());
        }
    }

    public class CreateGadgetFullCommandModelValidator : AbstractValidator<CreateGadgetFullRequestModel>
    {
        public CreateGadgetFullCommandModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Categories).NotEmpty();
            RuleForEach(x => x.Categories)
                .SetValidator(new CategoryRequestModelValidator());
        }
    }

    public class CategoryRequestModelValidator: AbstractValidator<CreateAssignCategoryRequestModel>
    {
        public CategoryRequestModelValidator()
        {
            RuleFor(x => x)
               .Custom((model, context) =>
               {
                   bool hasCategoryId = model.CategoryId.HasValue;
                   bool hasCategoryName = !string.IsNullOrWhiteSpace(model.CategoryName);

                   if (hasCategoryId && hasCategoryName)
                   {
                       context.AddFailure("You must provide either CategoryId OR CategoryName, not both.");
                   }
                   else if (!hasCategoryId && !hasCategoryName)
                   {
                       context.AddFailure("You must provide either CategoryId OR CategoryName.");
                   }
               });
        }
    }
    
    public class CreateGadgetFullCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<CreateGadgetFullCommand, BaseOutput<CreateGadgetFullResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<CreateGadgetFullResponseModel>> Handle(CreateGadgetFullCommand request, CancellationToken cancellationToken)
        {
            var gadget = new Gadget(request.GadgetId, request.Model.Name, request.Model.StockQuantity, request.Model.Description, (Guid)request.Context[Constants.ContextKeys.UserId]!);

            var newCategories = new List<Category>();
            var existingCategoryIds = new List<Guid>();

            var gadgetCategories = new List<GadgetCategory>();

            for (int i = 0; i < request.Model.Categories.Count; i++)
            {
                var category = request.Model.Categories[i];
                Guid id;
                
                if (category.CategoryId is null)
                {
                    id = Guid.NewGuid();
                    newCategories.Add(new Category(id,category.CategoryName, (Guid)request.Context[Constants.ContextKeys.UserId]!));
                }
                else
                {
                    id = category.CategoryId.Value;
                    existingCategoryIds.Add(id);
                }

                gadgetCategories.Add(new GadgetCategory(Guid.NewGuid(), gadget.Id, id, i, (Guid)request.Context[Constants.ContextKeys.UserId]!));

            }
            
            var transactionModel = await _repository.BeginTransactionAsync();
            var transaction = (IDbContextTransaction)transactionModel.Transaction!;

            try
            {
                await _repository.UpsertAsync(gadget).ConfigureAwait(false);
                
                cancellationToken.ThrowIfCancellationRequested();

                await _repository.AddRangeAsync(newCategories).ConfigureAwait(false);
                
                cancellationToken.ThrowIfCancellationRequested();
                
                await _repository.AddRangeAsync(gadgetCategories).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            var ret = new BaseOutput<CreateGadgetFullResponseModel>(OperationResult.Created, new CreateGadgetFullResponseModel()
            {
                Id = gadget.Id
            });
            
            ret.Model!.CategoriesCreatedIds.AddRange(newCategories.Select(x => x.Id));

            return ret;
        }
    }
}