namespace Clinical.Domain.Entities;

public sealed class PipelineWatermark
{
    public int Id { get; set; }
    public string PipelineName { get; set; } = string.Empty;
    public DateTimeOffset LastProcessedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
