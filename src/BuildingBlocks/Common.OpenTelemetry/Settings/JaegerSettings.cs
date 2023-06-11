namespace Common.OpenTelemetry.Settings;

public class JaegerSettings
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string ServiceName { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
}