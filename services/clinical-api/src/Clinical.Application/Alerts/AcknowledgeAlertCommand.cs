using MediatR;

namespace Clinical.Application.Alerts;

public sealed record AcknowledgeAlertCommand(Guid AlertId) : IRequest<AcknowledgeAlertResult>;

public sealed record AcknowledgeAlertResult(
    Guid AlertId,
    string Status,
    DateTimeOffset? AcknowledgedAtUtc);
