namespace Clinical.Domain.Entities;

public sealed class Alert
{
    public Guid Id { get; set; }
    public Guid EncounterId { get; set; }
    public Guid PredictionRunId { get; set; }
    public double Threshold { get; set; }
    public double RiskScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? AcknowledgedAtUtc { get; set; }
    public DateTimeOffset? ClosedAtUtc { get; set; }

    public PredictionRun? PredictionRun { get; set; }
}
