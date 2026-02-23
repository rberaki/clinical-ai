namespace Clinical.Domain.Entities;

public sealed class PredictionRun
{
    public Guid Id { get; set; }
    public Guid FeatureRunId { get; set; }
    public FeatureRun FeatureRun { get; set; } = default!;
    public string ModelVersion { get; set; } = default!;
    public int HorizonHours { get; set; }
    public double RiskScore { get; set; }
    public string? SurvivalSummaryJson { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
