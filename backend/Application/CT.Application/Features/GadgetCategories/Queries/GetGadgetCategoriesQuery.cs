using MediatR;
using Microsoft.EntityFrameworkCore;
using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Extensions;
using CT.Application.Abstractions.Models;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Interfaces;
using System.Diagnostics;

namespace CT.Application.Features.GadgetCategories.Queries;

public class GetGadgetCategoriesQueryResponseModel
{
    public GetGadgetCategoriesQueryResponseModel()
    {
    }

    public GetGadgetCategoriesQueryResponseModel(Guid id, Guid gadgetId, Guid categoryId, string categoryName, int ordinal, DateTimeOffset createdAt, DateTimeOffset updatedAt) : this()
    {
        Id = id;
        GadgetId = gadgetId;
        CategoryId = categoryId;
        CategoryName = categoryName;
        Ordinal = ordinal;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; set; }
    public Guid GadgetId { get; set; }
    public Guid CategoryId { get; set; }
    public int Ordinal { get; set; }
    public string CategoryName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class GetGadgetCategoriesQuery : ContextualRequest, IRequest<GetEntitiesResponse<GetGadgetCategoriesQueryResponseModel>>
{
    public PagingQueryParameters? PagingParameters { get; set; }
    public FilterQueryParameters? FilterParameters { get; set; }
    public SortQueryParameters? SortParameters { get; set; }
}

public class GetGadgetCategoriesQueryHandler(IGadgetsRepositoryService repository) : IRequestHandler<GetGadgetCategoriesQuery, GetEntitiesResponse<GetGadgetCategoriesQueryResponseModel>>
{
    private readonly IGadgetsRepositoryService _repository = repository;

    public async Task<GetEntitiesResponse<GetGadgetCategoriesQueryResponseModel>> Handle(GetGadgetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = GetQuery(request.FilterParameters, request.SortParameters);

        cancellationToken.ThrowIfCancellationRequested();

        var data = await _repository.QueryAsync(query, pageIndex: request.PagingParameters?.PageIndex ?? 0, pageSize: request.PagingParameters?.PageSize ?? -1)
                            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        var response = new GetEntitiesResponse<GetGadgetCategoriesQueryResponseModel>(data.Records!, data.TotalRecordCount, data.PageIndex, data.PageSize);

        return response;
    }

    private IQueryable<GetGadgetCategoriesQueryResponseModel> GetQuery(FilterQueryParameters? filterParameters, SortQueryParameters? sortParameters)
    {
        var parGadgetId = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("gadgetId", StringComparison.CurrentCultureIgnoreCase));
        var gadgetId = parGadgetId?.GetFilterQueryParameterDeconstructed((value) => value is null ? (Guid?)null : new Guid((string)value));

        var parCategoryId = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("categoryId", StringComparison.CurrentCultureIgnoreCase));
        var categoryId = parCategoryId?.GetFilterQueryParameterDeconstructed((value) => value is null ? (Guid?)null : new Guid((string)value));

        var parOrdinal = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("ordinal", StringComparison.CurrentCultureIgnoreCase));
        var ordinal = parCategoryId?.GetFilterQueryParameterDeconstructed((value) => (int?)value);

        var parCategoryName = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("categoryName", StringComparison.CurrentCultureIgnoreCase));
        var categoryName = parCategoryName?.GetFilterQueryParameterDeconstructed((value) => (string?)value);

        var parCreatedAt = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("createdAt", StringComparison.CurrentCultureIgnoreCase));
        var createdAt = parCreatedAt?.GetFilterQueryParameterDeconstructed((value) => ((string?)value)?.ToDateOnly().ToDateTimeOffset());

        var parUpdatedAt = filterParameters?.FirstOrDefault(x => x.FieldName.Equals("updatedAt", StringComparison.CurrentCultureIgnoreCase));
        var updatedAt = parUpdatedAt?.GetFilterQueryParameterDeconstructed((value) => ((string?)value)?.ToDateOnly().ToDateTimeOffset());

        var ctx = _repository.DbContext;

        var query =
            from
                gc in ctx.GadgetCategory
            join c in ctx.Category on gc.CategoryId equals c.Id
            where
                (categoryId == null || categoryId.Eq == null || Equals(categoryId.Eq, gc.CategoryId)) &&
                (gadgetId == null || gadgetId.Eq == null || Equals(gadgetId.Eq, gc.GadgetId)) &&

                (categoryName == null || categoryName.Eq == null || Equals(categoryName.Eq, c.Name)) &&
                (categoryName == null || categoryName.Gt == null || c.Name.CompareTo(categoryName.Gt) > 0) &&
                (categoryName == null || categoryName.Lt == null || c.Name.CompareTo(categoryName.Lt) < 0) &&
                (categoryName == null || categoryName.Gte == null || c.Name.CompareTo(categoryName.Gte) >= 0) &&
                (categoryName == null || categoryName.Lte == null || c.Name.CompareTo(categoryName.Lte) >= 0) &&
                (categoryName == null || categoryName.StartsWith == null || c.Name.StartsWith(categoryName.StartsWith)) &&
                (categoryName == null || categoryName.Contains == null || c.Name.Contains(categoryName.Contains)) &&

                (createdAt == null || createdAt.Eq == null || Equals(createdAt.Eq, gc.CreatedAt)) &&
                (createdAt == null || createdAt.Gt == null || gc.CreatedAt > createdAt.Gt) &&
                (createdAt == null || createdAt.Lt == null || gc.CreatedAt < createdAt.Lt) &&
                (createdAt == null || createdAt.Gte == null || gc.CreatedAt >= createdAt.Gte) &&
                (createdAt == null || createdAt.Lte == null || gc.CreatedAt >= createdAt.Lte) &&

                (updatedAt == null || updatedAt.Eq == null || Equals(updatedAt.Eq, gc.UpdatedAt)) &&
                (updatedAt == null || updatedAt.Gt == null || gc.UpdatedAt > updatedAt.Gt) &&
                (updatedAt == null || updatedAt.Lt == null || gc.UpdatedAt < updatedAt.Lt) &&
                (updatedAt == null || updatedAt.Gte == null || gc.UpdatedAt >= updatedAt.Gte) &&
                (updatedAt == null || updatedAt.Lte == null || gc.UpdatedAt >= updatedAt.Lte)
            select new GetGadgetCategoriesQueryResponseModel
            {
                Id = gc.Id,
                UpdatedAt = gc.UpdatedAt,
                CreatedAt = gc.CreatedAt,
                GadgetId = gc.GadgetId,
                CategoryId = gc.CategoryId,
                Ordinal = gc.Ordinal,
                CategoryName = c.Name
            };

        if (Debugger.IsAttached)
        {
            var qstring = query.ToQueryString();
        }

        sortParameters ??= [
            new SortQueryParameter("gadgetId", SortDirection.Asc),
            new SortQueryParameter("ordinal", SortDirection.Asc)];

        query = query.OrderBySortParameters(sortParameters);

        return query;
    }
}
