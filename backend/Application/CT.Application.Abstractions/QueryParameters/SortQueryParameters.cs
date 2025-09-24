using CT.Application.Abstractions.Enums;

namespace CT.Application.Abstractions.QueryParameters;

public class SortQueryParameters : List<SortQueryParameter>
{
}

public class SortQueryParameter(string fieldName, SortDirection direction)
{
    public string FieldName { get; set; } = fieldName;
    public SortDirection Direction { get; set; } = direction;
}
