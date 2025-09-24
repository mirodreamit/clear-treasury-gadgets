using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Interfaces;

namespace CT.Application.Abstractions.Models;

public class BaseOutput<T> : IBaseOutput
{
    public OperationResult Result { get; set; }
    public string? Message { get; set; }
    public object? Error { get; set; }

    public BaseOutput(OperationResult result, T model)
    {
        Result = result;
        Model = model;
    }
    public BaseOutput(T model): this(OperationResult.Ok, model) { }
    
    public BaseOutput(OperationResult result, string message, object error)
    {
        Result = result;
        Message = message;
        SetErrorObject(error);
    }

    public T? Model { get; set; }
 
    protected virtual void SetErrorObject(object error)
    {
        Error = error;
    }
}