using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;
using CT.Application.Abstractions.Enums;

namespace CT.Application.Features.GadgetCategories.Queries;

public class GetGadgetCategoryByIdQueryResponseModel
{
    public Guid Id { get; set; }
    public Guid GadgetId { get; set; }
    public Guid CategoryId { get; set; }
    public int Ordinal { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class GetGadgetCategoryByIdQuery(Guid GadgetCategoryId) : ContextualRequest, IRequest<BaseOutput<GetGadgetCategoryByIdQueryResponseModel?>>
{
    //public List<string> RequiredClaims => new() { "entities.GetGadgetCategoryByIding.read" };

    public Guid GadgetCategoryId { get; set; } = GadgetCategoryId;
}

public class GetGadgetCategoryByIdQueryHandler(IGadgetsRepositoryService repository) : IRequestHandler<GetGadgetCategoryByIdQuery, BaseOutput<GetGadgetCategoryByIdQueryResponseModel?>>
{
    private readonly IGadgetsRepositoryService _repository = repository;

    public async Task<BaseOutput<GetGadgetCategoryByIdQueryResponseModel?>> Handle(GetGadgetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetEntityByIdAsync<GadgetCategory>(request.GadgetCategoryId).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        if (entity == null)
        {
            return new BaseOutput<GetGadgetCategoryByIdQueryResponseModel?>(OperationResult.NotFound, null);
        }

        GetGadgetCategoryByIdQueryResponseModel? res = null;

        if (entity != null)
        {
            res = new GetGadgetCategoryByIdQueryResponseModel
            {
                Id = entity.Id,
                GadgetId = entity.GadgetId,
                CategoryId = entity.CategoryId,
                CreatedAt = entity.CreatedAt,
                Ordinal = entity.Ordinal,
                UpdatedAt = entity.UpdatedAt
            };
        }

        var response = new BaseOutput<GetGadgetCategoryByIdQueryResponseModel?>(res!);

        return response;
    }
}
