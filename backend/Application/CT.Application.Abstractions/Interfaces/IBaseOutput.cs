using CT.Application.Abstractions.Enums;

namespace CT.Application.Abstractions.Interfaces;

public interface IBaseOutput
{
    OperationResult Result { get; set; }
    string? Message { get; set; }
    object? Error { get; set; }
}

