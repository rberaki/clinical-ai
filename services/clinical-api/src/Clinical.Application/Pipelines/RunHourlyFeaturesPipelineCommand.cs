using MediatR;

namespace Clinical.Application.Pipelines;

public sealed record RunHourlyFeaturesPipelineCommand() : IRequest<RunHourlyFeaturesPipelineResult>;

public sealed record RunHourlyFeaturesPipelineResult(
    IReadOnlyList<Guid> RunIds,
    int EventsProcessed,
    int EncountersProcessed,
    DateTimeOffset WindowStartUtc,
    DateTimeOffset WindowEndUtc,
    DateTimeOffset NewWatermarkUtc
);
