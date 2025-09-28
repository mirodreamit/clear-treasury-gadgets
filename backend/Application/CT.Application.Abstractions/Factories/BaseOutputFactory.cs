using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Interfaces;

namespace CT.Application.Abstractions.Factories;
public static class BaseOutputFactory 
{
    public static TResponse CreateError<TResponse>(
        OperationResult result,
        string message,
        object error)
        where TResponse : IBaseOutput
    {
        return (TResponse)Activator.CreateInstance(
            typeof(TResponse)!,
            result,
            message,
            error
        )!;
    }
}
