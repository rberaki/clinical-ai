using Clinical.Application.Events;
using Clinical.Domain.Entities;
using Clinical.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Clinical.Application.Handlers;

public sealed class IngestRawEventCommandHandler(ClinicalDbContext dbContext)
    : IRequestHandler<IngestRawEventCommand, IngestRawEventResult>
{
    private readonly ClinicalDbContext _dbContext = dbContext;

    public async Task<IngestRawEventResult> Handle(IngestRawEventCommand request, CancellationToken cancellationToken)
    {
        var hasIdempotencyKey = !string.IsNullOrWhiteSpace(request.IdempotencyKey);

        if (hasIdempotencyKey)
        {
            var existing = await _dbContext.RawEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdempotencyKey == request.IdempotencyKey, cancellationToken);

            if (existing is not null)
            {
                return new IngestRawEventResult(existing.Id, true);
            }
        }

        var rawEvent = new RawEvent
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = hasIdempotencyKey ? request.IdempotencyKey : null,
            PatientId = request.PatientId,
            EncounterId = request.EncounterId,
            SourceSystem = request.SourceSystem,
            EventType = request.EventType,
            PayloadJson = request.PayloadJson,
            OccurredAtUtc = request.OccurredAtUtc,
            IngestedAtUtc = DateTimeOffset.UtcNow,
        };

        _dbContext.RawEvents.Add(rawEvent);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new IngestRawEventResult(rawEvent.Id, false);
        }
        catch (DbUpdateException ex) when (
            hasIdempotencyKey &&
            ex.InnerException is PostgresException pg &&
            pg.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // Race condition: another request inserted with same idempotency key.
            var duplicate = await _dbContext.RawEvents
                .AsNoTracking()
                .FirstAsync(x => x.IdempotencyKey == request.IdempotencyKey, cancellationToken);

            return new IngestRawEventResult(duplicate.Id, true);
        }
    }
}
