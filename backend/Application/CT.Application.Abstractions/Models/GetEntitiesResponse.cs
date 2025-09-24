using CT.Application.Abstractions.Enums;

namespace CT.Application.Abstractions.Models;

public class GetEntitiesResponse<TResponseModel> : BaseOutput<List<TResponseModel>>
{
    public long TotalRecordCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public GetEntitiesResponse(List<TResponseModel> model) : base(model)
    {
        Result = OperationResult.Ok;
    }
    
    public GetEntitiesResponse(List<TResponseModel> model, long totalRecordCount, int pageIndex, int pageSize) : this(model)
    {
        TotalRecordCount = totalRecordCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public GetEntitiesResponse(OperationResult result, string message, object error) : base(result, message, error)
    {
    }
}
