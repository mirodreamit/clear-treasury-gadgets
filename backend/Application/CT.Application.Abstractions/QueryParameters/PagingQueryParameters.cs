namespace CT.Application.Abstractions.QueryParameters;

public class PagingQueryParameters
{
    public int PageSize { get; set; } = -1;
    public int PageIndex { get; set; } = 0;
}
