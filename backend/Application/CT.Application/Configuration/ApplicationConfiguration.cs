namespace CT.Application.Configuration;

public class ApplicationConfiguration
{
    public RequestProcessingConfiguration? RequestProcessingConfiguration { get; set; }
    public string ExporterPluginsFolder { get; set; }
}

public class RequestProcessingConfiguration
{
    public int WarningThresholdMiliseconds { get; set; } = 500;
}