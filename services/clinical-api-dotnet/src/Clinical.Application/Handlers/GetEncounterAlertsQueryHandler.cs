using Clinical.Application.Alerts;
using Clinical.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Handlers;

public sealed class GetEncounterAlertsQueryHandler(ClinicalDbContext dbContext)
    : IRequestHandler<GetEncounterAlertsQuery, IReadOnlyList<EncounterAlertDto>>
{
    private readonly ClinicalDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<EncounterAlertDto>> Handle(
        GetEncounterAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var status = string.IsNullOrWhiteSpace(request.Status) ? "Active" : request.Status;

        return await _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.EncounterId == request.EncounterId && x.Status == status)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new EncounterAlertDto(
                x.Id,
                x.EncounterId,
                x.PredictionRunId,
                x.Threshold,
                x.RiskScore,
                x.Status,
                x.CreatedAtUtc,
                x.AcknowledgedAtUtc,
                x.ClosedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
