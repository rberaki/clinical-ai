namespace Clinical.Domain.Entities;

public sealed class FeatureRun
{
    public Guid Id { get; set; }
    public string PipelineName { get; set; } = string.Empty;
    public Guid EncounterId { get; set; }
    public DateTimeOffset WindowStartUtc { get; set; }
    public DateTimeOffset WindowEndUtc { get; set; }
    public string FeatureVersion { get; set; } = string.Empty;
    public Guid[] InputEventIds { get; set; } = [];
    public string FeatureHash { get; set; } = string.Empty;
    public DateTimeOffset StartedAtUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ProcessedEvents { get; set; }
}
