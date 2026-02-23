using Clinical.Domain.Entities;

namespace Clinical.Application.Models;

public interface IRiskModel
{
    Task<RiskPrediction> PredictAsync(FeatureRun featureRun, int horizonHours, CancellationToken ct);
}

public sealed record RiskPrediction(
    double RiskScore,
    string ModelVersion,
    string? SurvivalSummaryJson);
