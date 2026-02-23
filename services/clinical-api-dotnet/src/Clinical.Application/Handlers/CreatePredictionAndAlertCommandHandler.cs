using Clinical.Application.Models;
using Clinical.Application.Predictions;
using Clinical.Domain.Entities;
using Clinical.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Handlers;

public sealed class CreatePredictionAndAlertCommandHandler(
    ClinicalDbContext dbContext,
    IRiskModel riskModel) : IRequestHandler<CreatePredictionAndAlertCommand, CreatePredictionAndAlertResult>
{
    private readonly ClinicalDbContext _dbContext = dbContext;
    private readonly IRiskModel _riskModel = riskModel;

    public async Task<CreatePredictionAndAlertResult> Handle(
        CreatePredictionAndAlertCommand request,
        CancellationToken cancellationToken)
    {
        var featureRun = await _dbContext.FeatureRuns
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.FeatureRunId, cancellationToken)
            ?? throw new KeyNotFoundException($"FeatureRun '{request.FeatureRunId}' was not found.");

        var prediction = await _riskModel.PredictAsync(featureRun, request.HorizonHours, cancellationToken);

        var createdAt = DateTimeOffset.UtcNow;

        var predictionRun = new PredictionRun
        {
            Id = Guid.NewGuid(),
            FeatureRunId = featureRun.Id,
            ModelVersion = prediction.ModelVersion,
            HorizonHours = request.HorizonHours,
            RiskScore = prediction.RiskScore,
            SurvivalSummaryJson = prediction.SurvivalSummaryJson,
            CreatedAtUtc = createdAt
        };

        _dbContext.PredictionRuns.Add(predictionRun);

        Alert? alert = null;

        if (prediction.RiskScore >= request.Threshold)
        {
            alert = new Alert
            {
                Id = Guid.NewGuid(),
                EncounterId = featureRun.EncounterId,
                PredictionRunId = predictionRun.Id,
                Threshold = request.Threshold,
                RiskScore = prediction.RiskScore,
                Status = "Active",
                CreatedAtUtc = createdAt
            };

            _dbContext.Alerts.Add(alert);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreatePredictionAndAlertResult(
            predictionRun.Id,
            alert?.Id,
            prediction.RiskScore,
            request.Threshold);
    }
}
