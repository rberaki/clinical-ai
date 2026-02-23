using System.Security.Cryptography;
using System.Text;
using Clinical.Application.Pipelines;
using Clinical.Domain.Entities;
using Clinical.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Handlers;

public sealed class RunHourlyFeaturesPipelineCommandHandler(ClinicalDbContext dbContext)
    : IRequestHandler<RunHourlyFeaturesPipelineCommand, RunHourlyFeaturesPipelineResult>
{
    private const string PipelineName = "features_hourly";
    private static readonly DateTimeOffset Epoch = DateTimeOffset.FromUnixTimeSeconds(0);

    private readonly ClinicalDbContext _dbContext = dbContext;

    public async Task<RunHourlyFeaturesPipelineResult> Handle(
        RunHourlyFeaturesPipelineCommand request,
        CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var watermark = await _dbContext.PipelineWatermarks
            .FirstOrDefaultAsync(x => x.PipelineName == PipelineName, cancellationToken);

        var lastProcessed = watermark?.LastProcessedAtUtc ?? Epoch;

        // Pull NEW events since watermark, based on OccurredAtUtc (source time), not IngestedAtUtc.
        var newEvents = await _dbContext.RawEvents
            .AsNoTracking()
            .Where(x => x.EncounterId != null && x.OccurredAtUtc > lastProcessed)
            .OrderBy(x => x.OccurredAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        if (newEvents.Count == 0)
        {
            // No-op: do not advance watermark
            await tx.CommitAsync(cancellationToken);
            return new RunHourlyFeaturesPipelineResult(
                Array.Empty<Guid>(),
                0,
                0,
                lastProcessed,
                lastProcessed,
                lastProcessed);
        }

        var groups = newEvents.GroupBy(e => e.EncounterId!.Value).ToList();
        var completedAt = DateTimeOffset.UtcNow;

        var runIds = new List<Guid>(groups.Count);

        foreach (var g in groups)
        {
            var encounterId = g.Key;
            var ordered = g.OrderBy(e => e.OccurredAtUtc).ThenBy(e => e.Id).ToList();

            var windowStartUtc = ordered.First().OccurredAtUtc;
            var windowEndUtc = ordered.Last().OccurredAtUtc;

            var inputIds = ordered.Select(e => e.Id).ToArray();
            var featureHash = ComputeStableHash(ordered);

            var run = new FeatureRun
            {
                Id = Guid.NewGuid(),
                PipelineName = PipelineName,
                EncounterId = encounterId,
                WindowStartUtc = windowStartUtc,
                WindowEndUtc = windowEndUtc,
                FeatureVersion = "v0",
                InputEventIds = inputIds,
                FeatureHash = featureHash,
                StartedAtUtc = startedAt,
                CompletedAtUtc = completedAt,
                Status = "Completed",
                ProcessedEvents = ordered.Count
            };

            _dbContext.FeatureRuns.Add(run);
            runIds.Add(run.Id);
        }

        var newWatermarkUtc = newEvents.Max(e => e.OccurredAtUtc);

        if (watermark is null)
        {
            _dbContext.PipelineWatermarks.Add(new PipelineWatermark
            {
                PipelineName = PipelineName,
                LastProcessedAtUtc = newWatermarkUtc,
                UpdatedAtUtc = completedAt
            });
        }
        else
        {
            watermark.LastProcessedAtUtc = newWatermarkUtc;
            watermark.UpdatedAtUtc = completedAt;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return new RunHourlyFeaturesPipelineResult(
            runIds,
            newEvents.Count,
            groups.Count,
            lastProcessed,
            newWatermarkUtc,
            newWatermarkUtc);
    }

    private static string ComputeStableHash(IEnumerable<RawEvent> events)
    {
        var sb = new StringBuilder();
        foreach (var e in events.OrderBy(x => x.OccurredAtUtc).ThenBy(x => x.Id))
        {
            sb.Append(e.Id).Append('|')
              .Append(e.OccurredAtUtc.ToString("O")).Append('|')
              .Append(e.EventType).Append('\n');
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
