namespace MakeWish.UserService.Telemetry;

public sealed class OtelOptions
{
    public const string SectionName = "Otel";

    public required bool IsEnabled { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public string MeterName { get; init; } = string.Empty;
}