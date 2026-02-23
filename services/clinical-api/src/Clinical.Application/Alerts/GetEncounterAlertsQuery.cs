using MediatR;

namespace Clinical.Application.Alerts;

public sealed record GetEncounterAlertsQuery(
    Guid EncounterId,
    string? Status) : IRequest<IReadOnlyList<EncounterAlertDto>>;

public sealed record EncounterAlertDto(
    Guid Id,
    Guid EncounterId,
    Guid PredictionRunId,
    double Threshold,
    double RiskScore,
    string Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? AcknowledgedAtUtc,
    DateTimeOffset? ClosedAtUtc);
