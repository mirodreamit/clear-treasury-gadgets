using MediatR;
using CT.Application.Abstractions.Extensions;
using CT.Application.Abstractions.Models;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Interfaces;

namespace CT.Application.Features.Categories.Queries;

public class GetCategoriesQueryResponseModel
{
    public GetCategoriesQueryResponseModel()
    { 
    }

    public GetCategoriesQueryResponseModel(Guid id, string name, DateTimeOffset createdAt, DateTimeOffset updatedAt): this()
    { 
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class GetCategoriesQuery : ContextualRequest, IRequest<GetEntitiesResponse<GetCategoriesQueryResponseModel>>
{
    //public List<string> RequiredClaims => new() { "entities.GetCategoriesing.read" };

    public PagingQueryParameters? PagingParameters { get; set; }
    public FilterQueryParameters? FilterParameters { get; set; }
    public SortQueryParameters? SortParameters { get; set; }
}

public class GetCategoriesQueryHandler(IGadgetsRepositoryService repository) : IRequestHandler<GetCategoriesQuery, GetEntitiesResponse<GetCategoriesQueryResponseModel>>
{
    private readonly IGadgetsRepositoryService _repository = repository;

    public async Task<GetEntitiesResponse<GetCategoriesQueryResponseModel>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = GetQuery(request.FilterParameters, request.SortParameters);

        cancellationToken.ThrowIfCancellationRequested();

        var data = await _repository.QueryAsync(query, pageIndex: request.PagingParameters?.PageIndex ?? 0, pageSize: request.PagingParameters?.PageSize ?? -1)
                            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        var response = new GetEntitiesResponse<GetCategoriesQueryResponseModel>(data.Records!, data.TotalRecordCount, data.PageIndex, data.PageSize);

        return response;
    }

    private IQueryable<GetCategoriesQueryResponseModel> GetQuery(FilterQueryParameters? filterParameters, SortQueryParameters? sortParameters)
    {
        var parName = filterParameters?.FirstOrDefault(x => x.FieldName == "name");
        var name = parName?.GetFilterQueryParameterDeconstructed((value) => (string?)value);

        var parCreatedAt = filterParameters?.FirstOrDefault(x => x.FieldName == "createdAt");
        var createdAt = parCreatedAt?.GetFilterQueryParameterDeconstructed((value) => ((string?)value)?.ToDateOnly().ToDateTimeOffset());

        var parUpdatedAt = filterParameters?.FirstOrDefault(x => x.FieldName == "updatedAt");
        var updatedAt = parUpdatedAt?.GetFilterQueryParameterDeconstructed((value) => ((string?)value)?.ToDateOnly().ToDateTimeOffset());

        var ctx = _repository.DbContext;

        var query = 
            from 
                c in ctx.Category
            where
                (name == null || name.Eq == null || Equals(name.Eq, c.Name)) &&
                (name == null || name.Gt == null || c.Name.CompareTo(name.Gt) > 0) &&
                (name == null || name.Lt == null || c.Name.CompareTo(name.Lt) < 0) &&
                (name == null || name.Gte == null || c.Name.CompareTo(name.Gte) >= 0) &&
                (name == null || name.Lte == null || c.Name.CompareTo(name.Lte) >= 0) &&
                (name == null || name.StartsWith == null || c.Name.StartsWith(name.StartsWith)) &&
                (name == null || name.Contains == null || c.Name.Contains(name.Contains)) &&
              
                (createdAt == null || createdAt.Eq == null || Equals(createdAt.Eq, c.CreatedAt)) &&
                (createdAt == null || createdAt.Gt == null || c.CreatedAt > createdAt.Gt) &&
                (createdAt == null || createdAt.Lt == null || c.CreatedAt < createdAt.Lt) &&
                (createdAt == null || createdAt.Gte == null || c.CreatedAt >= createdAt.Gte) &&
                (createdAt == null || createdAt.Lte == null || c.CreatedAt >= createdAt.Lte) &&

                (updatedAt == null || updatedAt.Eq == null || Equals(updatedAt.Eq, c.UpdatedAt)) &&
                (updatedAt == null || updatedAt.Gt == null || c.UpdatedAt > updatedAt.Gt) &&
                (updatedAt == null || updatedAt.Lt == null || c.UpdatedAt < updatedAt.Lt) &&
                (updatedAt == null || updatedAt.Gte == null || c.UpdatedAt >= updatedAt.Gte) &&
                (updatedAt == null || updatedAt.Lte == null || c.UpdatedAt >= updatedAt.Lte)
            select new GetCategoriesQueryResponseModel
            {
                Id = c.Id,
                UpdatedAt = c.UpdatedAt,
                CreatedAt = c.CreatedAt,
                Name = c.Name
            };

        sortParameters ??= [new SortQueryParameter("updatedAt", Abstractions.Enums.SortDirection.Desc)];

        query = query.OrderBySortParameters(sortParameters);

        return query;
    }
}
