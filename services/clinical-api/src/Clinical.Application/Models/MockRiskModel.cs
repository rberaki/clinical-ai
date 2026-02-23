using Clinical.Domain.Entities;

namespace Clinical.Application.Models;

public sealed class MockRiskModel : IRiskModel
{
    public Task<RiskPrediction> PredictAsync(FeatureRun featureRun, int horizonHours, CancellationToken ct)
    {
        var eventsFactor = Math.Clamp(featureRun.ProcessedEvents / 20.0, 0.0, 1.0);
        var horizonFactor = Math.Clamp(horizonHours / 48.0, 0.0, 1.0) * 0.1;
        var risk = Math.Clamp(eventsFactor + horizonFactor, 0.0, 1.0);

        var prediction = new RiskPrediction(
            RiskScore: risk,
            ModelVersion: "mock-v0",
            SurvivalSummaryJson: $"{{\"horizonHours\":{horizonHours},\"riskScore\":{risk:F4}}}");

        return Task.FromResult(prediction);
    }
}
