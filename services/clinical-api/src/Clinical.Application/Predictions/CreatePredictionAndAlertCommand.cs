using MediatR;

namespace Clinical.Application.Predictions;

public sealed record CreatePredictionAndAlertCommand(
    Guid FeatureRunId,
    int HorizonHours,
    double Threshold) : IRequest<CreatePredictionAndAlertResult>;

public sealed record CreatePredictionAndAlertResult(
    Guid PredictionRunId,
    Guid? AlertId,
    double RiskScore,
    double Threshold);
