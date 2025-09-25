﻿using CT.Application.Abstractions.Enums;

namespace CT.Application.Models;

public class RegisterUserResponse
{
    public string? UserIdentifier { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? Message { get; set; }

    public RegisterUserResult Result { get; set; }
    public string ResultDescription
    {
        get
        {
            return Enum.GetName(typeof(RegisterUserResult), Result)!;
        }
    }
}
