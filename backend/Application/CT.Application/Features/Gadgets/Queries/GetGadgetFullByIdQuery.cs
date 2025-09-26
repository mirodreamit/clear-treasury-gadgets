using MediatR;
using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Extensions;
using CT.Application.Abstractions.Models;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Interfaces;

namespace CT.Application.Features.Gadgets.Queries;

#region models
public class GetGadgetFullByIdQueryResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid LastModifiedByUserId { get; set; }
    public string LastModifiedDisplayName { get; set; }
    public GadgetCategoryPagingResponseModel GadgetCategories { get; set; }
}

public class GadgetCategoryPagingResponseModel
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long TotalRecordCount { get; set; }

    public List<GadgetCategoryResponseModel> Items { get; set; }
}

public class GadgetCategoryResponseModel
{
    public Guid Id { get; set; }
    public Guid GadgetId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int Ordinal { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
#endregion

public class GetGadgetFullByIdQuery(Guid gadgetId) : ContextualRequest, IRequest<BaseOutput<GetGadgetFullByIdQueryResponseModel>>
{
    //public List<string> RequiredClaims => new() { "entities.GetGadgetFullByIding.read" };

    public Guid GadgetId { get; set; } = gadgetId;

    public PagingQueryParameters? PagingParameters { get; set; }
    public FilterQueryParameters? FilterParameters { get; set; }
    public SortQueryParameters? SortParameters { get; set; }
}

public class GetGadgetFullByIdQueryHandler(IGadgetsRepositoryService repository) : IRequestHandler<GetGadgetFullByIdQuery, BaseOutput<GetGadgetFullByIdQueryResponseModel>>
{
    private readonly IGadgetsRepositoryService _repository = repository;

    public async Task<BaseOutput<GetGadgetFullByIdQueryResponseModel>> Handle(GetGadgetFullByIdQuery request, CancellationToken cancellationToken)
    {
        var ctx = _repository.DbContext;

        var queryGadget =
            from g in ctx.Gadget
            join u in ctx.User on g.LastModifiedByUserId equals u.Id
            where
                g.Id == request.GadgetId
            select new 
            {
                g.Id,
                g.CreatedAt,
                g.UpdatedAt,
                g.LastModifiedByUserId,
                g.Name,
                g.StockQuantity,
                g.Description,
                LastModifiedDisplayName = u.DisplayName
            };

        cancellationToken.ThrowIfCancellationRequested();

        var gadgetData = await _repository.QueryAsync(queryGadget);

        var gadgetView = gadgetData.Records!.FirstOrDefault();

        cancellationToken.ThrowIfCancellationRequested();

        if (gadgetView == null)
        {
            return new BaseOutput<GetGadgetFullByIdQueryResponseModel>(OperationResult.NotFound, null!);
        }

        var query = GetQuery(request.GadgetId, request.SortParameters);

        cancellationToken.ThrowIfCancellationRequested();

        var data = await _repository.QueryAsync(query, pageIndex: request.PagingParameters?.PageIndex ?? 0, pageSize: request.PagingParameters?.PageSize ?? -1)
                            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        var response = new BaseOutput<GetGadgetFullByIdQueryResponseModel>(new GetGadgetFullByIdQueryResponseModel
        {
            Id = request.GadgetId,
            Name = gadgetView!.Name,
            StockQuantity = gadgetView!.StockQuantity,
            CreatedAt = gadgetView!.CreatedAt,
            UpdatedAt = gadgetView!.UpdatedAt,
            Description = gadgetView!.Description,
            LastModifiedByUserId = gadgetView!.LastModifiedByUserId,
            LastModifiedDisplayName = gadgetView!.LastModifiedDisplayName,
            GadgetCategories = new GadgetCategoryPagingResponseModel
            { 
                Items = data.Records!,
                PageIndex = data.PageIndex,
                PageSize = data.PageSize,
                TotalRecordCount = data.TotalRecordCount
            }
        });

        return response;
    }

    #region private methods

    private IQueryable<GadgetCategoryResponseModel> GetQuery(Guid gadgetId, SortQueryParameters? sortParameters)
    {
        var ctx = _repository.DbContext;

        var query =
            from
                gc in ctx.GadgetCategory
            join 
                c in ctx.Category on gc.CategoryId equals c.Id
            where
                Equals(gadgetId, gc.GadgetId)
            select new GadgetCategoryResponseModel
            {
                Id = gc.Id,
                GadgetId = gc.GadgetId,
                CategoryId = gc.CategoryId,
                Ordinal = gc.Ordinal,
                CategoryName = c.Name,
                CreatedAt = gc.CreatedAt,
                UpdatedAt = gc.UpdatedAt
            };

        sortParameters ??= [
            new SortQueryParameter("ordinal", SortDirection.Asc)];

        query = query.OrderBySortParameters(sortParameters);

        return query;
    }
    #endregion
}



