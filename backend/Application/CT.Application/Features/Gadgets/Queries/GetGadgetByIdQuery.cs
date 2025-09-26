using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;

namespace CT.Application.Features.Gadgets.Queries;

public class GetGadgetByIdQueryResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description  { get; set; }
    public int StockQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid LastModifiedByUserId { get; set; }
    public string LastModifiedByUserDisplayName { get; set; }
}

public class GetGadgetByIdQuery(Guid GadgetId) : ContextualRequest, IRequest<BaseOutput<GetGadgetByIdQueryResponseModel>>
{
    //public List<string> RequiredClaims => new() { "entities.GetGadgetByIding.read" };

    public Guid GadgetId { get; set; } = GadgetId;
}

public class GetGadgetByIdQueryHandler(IGadgetsRepositoryService GadgetMakerRepositoryService) : IRequestHandler<GetGadgetByIdQuery, BaseOutput<GetGadgetByIdQueryResponseModel>>
{
    private readonly IGadgetsRepositoryService _repository = GadgetMakerRepositoryService;

    public async Task<BaseOutput<GetGadgetByIdQueryResponseModel>> Handle(GetGadgetByIdQuery request, CancellationToken cancellationToken)
    {
        var ctx = _repository.DbContext;

        var query =
            from g in ctx.Gadget
            join u in ctx.User on g.LastModifiedByUserId equals u.Id
            where
                g.Id == request.GadgetId
            select new GetGadgetByIdQueryResponseModel
            { 
                Id = g.Id,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                LastModifiedByUserId = g.LastModifiedByUserId,
                Name = g.Name,
                StockQuantity = g.StockQuantity,
                Description = g.Description,
                LastModifiedByUserDisplayName = u.DisplayName
            };

        cancellationToken.ThrowIfCancellationRequested();

        var view = await _repository.QueryAsync(query);

        var responseModel = view.Records!.FirstOrDefault();

        cancellationToken.ThrowIfCancellationRequested();

        if (responseModel == null)
        {
            return new BaseOutput<GetGadgetByIdQueryResponseModel>(Abstractions.Enums.OperationResult.NotFound, null!);
        }

        var response = new BaseOutput<GetGadgetByIdQueryResponseModel>(responseModel);

        return response;
    }
}