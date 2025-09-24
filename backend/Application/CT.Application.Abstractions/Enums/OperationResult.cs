namespace CT.Application.Abstractions.Enums;

public enum OperationResult
{
    Ok,

    Created,
    Updated,
    Deleted,
    NotFound,

    BadRequest,
    Unauthorized,
    Forbidden,
    
    InternalError
}