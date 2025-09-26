using MediatR;
using CT.Application.Abstractions.Extensions;
using CT.Application.Abstractions.Models;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Interfaces;

namespace CT.Application.Features.Gadgets.Queries;

public class GetGadgetsQueryResponseModel
{
    public GetGadgetsQueryResponseModel()
    {
    }

    public GetGadgetsQueryResponseModel(Guid id, string name, DateTimeOffset createdAt, DateTimeOffset updatedAt) : this()
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public int StockQuantity { get; set; }

    public string LastModifiedByUserDisplayName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class GetGadgetsQuery : ContextualRequest, IRequest<GetEntitiesResponse<GetGadgetsQueryResponseModel>>
{
    //public List<string> RequiredClaims => new() { "entities.GetGadgets.read" };

    public PagingQueryParameters? PagingParameters { get; set; }
    public FilterQueryParameters? FilterParameters { get; set; }
    public SortQueryParameters? SortParameters { get; set; }
}

public class GetGadgetsQueryHandler(IGadgetsRepositoryService repository) : IRequestHandler<GetGadgetsQuery, GetEntitiesResponse<GetGadgetsQueryResponseModel>>
{
    private readonly IGadgetsRepositoryService _repository = repository;

    public async Task<GetEntitiesResponse<GetGadgetsQueryResponseModel>> Handle(GetGadgetsQuery request, CancellationToken cancellationToken)
    {
        var query = GetQuery(request.FilterParameters, request.SortParameters);

        cancellationToken.ThrowIfCancellationRequested();

        var data = await _repository.QueryAsync(query, pageIndex: request.PagingParameters?.PageIndex ?? 0, pageSize: request.PagingParameters?.PageSize ?? -1)
                            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        var response = new GetEntitiesResponse<GetGadgetsQueryResponseModel>(data.Records!, data.TotalRecordCount, data.PageIndex, data.PageSize);

        return response;
    }

    private IQueryable<GetGadgetsQueryResponseModel> GetQuery(FilterQueryParameters? filterParameters, SortQueryParameters? sortParameters)
    {
        var parName = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("name", StringComparison.CurrentCultureIgnoreCase));
        var name = parName?.GetFilterQueryParameterDeconstructed((value) => (string?)value);

        var parLastModifiedByUserDisplayName = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("lastModifiedByUserDisplayName", StringComparison.CurrentCultureIgnoreCase));
        var lastModifiedByUserDisplayName = parLastModifiedByUserDisplayName?.GetFilterQueryParameterDeconstructed((value) => (string?)value);

        var parCreatedAt = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("createdAt", StringComparison.CurrentCultureIgnoreCase));
        var createdAt = parCreatedAt?.GetFilterQueryParameterDeconstructed((value) => ((string?)value)?.ToDateOnly().ToDateTimeOffset());

        var parUpdatedAt = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("updatedAt", StringComparison.CurrentCultureIgnoreCase));
        var updatedAt = parUpdatedAt?.GetFilterQueryParameterDeconstructed((value) => ((string?)value)?.ToDateOnly().ToDateTimeOffset());

        var parStockQuantity = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("stockQuantity", StringComparison.CurrentCultureIgnoreCase));
        var stockQuantity = parStockQuantity?.GetFilterQueryParameterDeconstructed((value) => (int?)value);

        var ctx = _repository.DbContext;

        var query =
            from
                g in ctx.Gadget
                join u in ctx.User on g.LastModifiedByUserId equals u.Id 
            where
                (name == null || name.Eq == null || Equals(name.Eq, g.Name)) &&
                (name == null || name.Gt == null || g.Name.CompareTo(name.Gt) > 0) &&
                (name == null || name.Lt == null || g.Name.CompareTo(name.Lt) < 0) &&
                (name == null || name.Gte == null || g.Name.CompareTo(name.Gte) >= 0) &&
                (name == null || name.Lte == null || g.Name.CompareTo(name.Lte) >= 0) &&
                (name == null || name.StartsWith == null || g.Name.StartsWith(name.StartsWith)) &&
                (name == null || name.Contains == null || g.Name.Contains(name.Contains)) &&

                (stockQuantity == null || stockQuantity.Eq == null || g.StockQuantity.CompareTo(stockQuantity.Eq) == 0) &&
                (stockQuantity == null || stockQuantity.Gt == null || g.StockQuantity.CompareTo(stockQuantity.Gt) > 0) &&
                (stockQuantity == null || stockQuantity.Lt == null || g.StockQuantity.CompareTo(stockQuantity.Lt) < 0) &&
                (stockQuantity == null || stockQuantity.Gte == null || g.StockQuantity.CompareTo(stockQuantity.Gte) >= 0) &&
                (stockQuantity == null || stockQuantity.Lte == null || g.StockQuantity.CompareTo(stockQuantity.Lte) >= 0) &&

                (lastModifiedByUserDisplayName == null || lastModifiedByUserDisplayName.Eq == null || Equals(lastModifiedByUserDisplayName.Eq, u.DisplayName)) &&
                (lastModifiedByUserDisplayName == null || lastModifiedByUserDisplayName.Gt == null || u.DisplayName.CompareTo(lastModifiedByUserDisplayName.Gt) > 0) &&
                (lastModifiedByUserDisplayName == null || lastModifiedByUserDisplayName.Lt == null || u.DisplayName.CompareTo(lastModifiedByUserDisplayName.Lt) < 0) &&
                (lastModifiedByUserDisplayName == null || lastModifiedByUserDisplayName.Gte == null || u.DisplayName.CompareTo(lastModifiedByUserDisplayName.Gte) >= 0) &&
                (lastModifiedByUserDisplayName == null || lastModifiedByUserDisplayName.Lte == null || u.DisplayName.CompareTo(lastModifiedByUserDisplayName.Lte) >= 0) &&
                (lastModifiedByUserDisplayName == null || lastModifiedByUserDisplayName.StartsWith == null || u.DisplayName.StartsWith(lastModifiedByUserDisplayName.StartsWith)) &&
                (lastModifiedByUserDisplayName == null || lastModifiedByUserDisplayName.Contains == null || u.DisplayName.Contains(lastModifiedByUserDisplayName.Contains)) &&

                (createdAt == null || createdAt.Eq == null || Equals(createdAt.Eq, g.CreatedAt)) &&
                (createdAt == null || createdAt.Gt == null || g.CreatedAt > createdAt.Gt) &&
                (createdAt == null || createdAt.Lt == null || g.CreatedAt < createdAt.Lt) &&
                (createdAt == null || createdAt.Gte == null || g.CreatedAt >= createdAt.Gte) &&
                (createdAt == null || createdAt.Lte == null || g.CreatedAt >= createdAt.Lte) &&

                (updatedAt == null || updatedAt.Eq == null || Equals(updatedAt.Eq, g.UpdatedAt)) &&
                (updatedAt == null || updatedAt.Gt == null || g.UpdatedAt > updatedAt.Gt) &&
                (updatedAt == null || updatedAt.Lt == null || g.UpdatedAt < updatedAt.Lt) &&
                (updatedAt == null || updatedAt.Gte == null || g.UpdatedAt >= updatedAt.Gte) &&
                (updatedAt == null || updatedAt.Lte == null || g.UpdatedAt >= updatedAt.Lte)
            select new GetGadgetsQueryResponseModel
            {
                Id = g.Id,
                UpdatedAt = g.UpdatedAt,
                CreatedAt = g.CreatedAt,
                Name = g.Name,
                LastModifiedByUserDisplayName = u.DisplayName,
                StockQuantity = g.StockQuantity
            };

        sortParameters ??= [new SortQueryParameter("updatedAt", Abstractions.Enums.SortDirection.Desc)];

        query = query.OrderBySortParameters(sortParameters);

        return query;
    }
}
