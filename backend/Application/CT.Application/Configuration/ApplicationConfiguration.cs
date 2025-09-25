namespace CT.Application.Configuration;

public class ApplicationConfiguration
{
    public RequestProcessingConfiguration? RequestProcessingConfiguration { get; set; }
    public TokenConfiguration TokenConfiguration { get; set; }
}

public class RequestProcessingConfiguration
{
    public int WarningThresholdMiliseconds { get; set; } = 500;
}

public class TokenConfiguration
{
    public string? Secret { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int AccessExpirationMins { get; set; }
    public int RefreshExpirationDays { get; set; }
}
