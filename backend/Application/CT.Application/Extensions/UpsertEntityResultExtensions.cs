using CT.Repository.Abstractions.Enums;
using CT.Application.Abstractions.Enums;

namespace CT.Application.Extensions;

public static class UpsertEntityResultExtensions
{
    public static OperationResult ToOperationResult(this UpsertEntityResult upsertEntityResult)
    {
        return upsertEntityResult switch
        {
            UpsertEntityResult.Updated => OperationResult.Updated,
            UpsertEntityResult.Inserted => OperationResult.Created,
            UpsertEntityResult.Unchanged => OperationResult.Ok,
            _ => throw new NotImplementedException(),
        };
    }
}
