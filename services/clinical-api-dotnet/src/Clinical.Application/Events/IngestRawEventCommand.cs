using MediatR;

namespace Clinical.Application.Events;

public sealed record IngestRawEventCommand(
    string? IdempotencyKey,
    Guid? PatientId,
    Guid? EncounterId,
    string SourceSystem,
    string EventType,
    string PayloadJson,
    DateTimeOffset OccurredAtUtc) : IRequest<IngestRawEventResult>;

public sealed record IngestRawEventResult(Guid RawEventId, bool IsDuplicate);
