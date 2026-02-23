using Clinical.Application.Alerts;
using Clinical.Domain;
using Clinical.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Handlers;

public sealed class AcknowledgeAlertCommandHandler(ClinicalDbContext dbContext)
    : IRequestHandler<AcknowledgeAlertCommand, AcknowledgeAlertResult>
{
    private readonly ClinicalDbContext _dbContext = dbContext;

    public async Task<AcknowledgeAlertResult> Handle(
        AcknowledgeAlertCommand request,
        CancellationToken cancellationToken)
    {
        var alert = await _dbContext.Alerts
            .FirstOrDefaultAsync(x => x.Id == request.AlertId, cancellationToken)
            ?? throw new KeyNotFoundException($"Alert '{request.AlertId}' was not found.");

        if (string.Equals(alert.Status, AlertStatus.Closed, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Closed alerts cannot be acknowledged.");
        }

        if (!string.Equals(alert.Status, AlertStatus.Acknowledged, StringComparison.Ordinal))
        {
            alert.Status = AlertStatus.Acknowledged;
            alert.AcknowledgedAtUtc = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new AcknowledgeAlertResult(alert.Id, alert.Status, alert.AcknowledgedAtUtc);
    }
}
