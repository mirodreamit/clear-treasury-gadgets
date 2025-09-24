using MediatR;
using CT.Application.Abstractions.Models;
using CT.Application.Interfaces;
using CT.Domain.Entities;

namespace CT.Application.Features.Gadgets.Queries;

public class GetGadgetByIdQueryResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int StockQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
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
        var entity = await _repository.GetEntityByIdAsync<Gadget>(request.GadgetId).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        if (entity == null)
        {
            return new BaseOutput<GetGadgetByIdQueryResponseModel>(Abstractions.Enums.OperationResult.NotFound, null!);
        }

        var response = new BaseOutput<GetGadgetByIdQueryResponseModel>(new GetGadgetByIdQueryResponseModel
        {
            Id = request.GadgetId,
            Name = entity!.Name,
            StockQuantity = entity!.StockQuantity,
            CreatedAt = entity!.CreatedAt,
            UpdatedAt = entity!.UpdatedAt
        });

        return response;
    }
}