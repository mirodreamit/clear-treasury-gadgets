using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using CT.Application.Abstractions.Enums;

namespace CT.Application.Features.Categories.Queries;

public class GetCategoryByIdQueryResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class GetCategoryByIdQuery(Guid categoryId) : ContextualRequest, IRequest<BaseOutput<GetCategoryByIdQueryResponseModel?>>
{
    //public List<string> RequiredClaims => new() { "entities.GetCategoryByIding.read" };

    public Guid CategoryId { get; set; } = categoryId;
}

public class GetCategoryByIdQueryHandler(IGadgetsRepositoryService repository) : IRequestHandler<GetCategoryByIdQuery, BaseOutput<GetCategoryByIdQueryResponseModel?>>
{
    private readonly IGadgetsRepositoryService _repository = repository;

    public async Task<BaseOutput<GetCategoryByIdQueryResponseModel?>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync<Category>(request.CategoryId).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        if (entity == null)
        {
            return new BaseOutput<GetCategoryByIdQueryResponseModel?>(OperationResult.NotFound, null);
        }

        var res = new GetCategoryByIdQueryResponseModel
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        
        var response = new BaseOutput<GetCategoryByIdQueryResponseModel?>(res!);

        return response;
    }
}