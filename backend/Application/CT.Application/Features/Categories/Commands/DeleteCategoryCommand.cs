using FluentValidation;
using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using static CT.Application.Features.Categories.Commands.DeleteCategoryCommand;
using CT.Application.Abstractions.Enums;

namespace CT.Application.Features.Categories.Commands;

public class DeleteCategoryCommand(Guid CategoryId) : ContextualRequest, IRequest<BaseOutput<DeleteCategoryResponseModel>>
{
    public Guid CategoryId { get; set; } = CategoryId;

    public class DeleteCategoryResponseModel
    {
        public Guid CategoryId { get; set; }
    }

    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }

    public class DeleteCategoryCommandHandler(IGadgetsRepositoryService repository) : IRequestHandler<DeleteCategoryCommand, BaseOutput<DeleteCategoryResponseModel>>
    {
        private readonly IGadgetsRepositoryService _repository = repository;

        public async Task<BaseOutput<DeleteCategoryResponseModel>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteHardAsync<Category>(request.CategoryId).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return new BaseOutput<DeleteCategoryResponseModel>(OperationResult.Deleted, new DeleteCategoryResponseModel() { CategoryId = request.CategoryId });
        }
    }
}
