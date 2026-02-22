namespace Clinical.Domain.Entities;

public sealed class RawEvent
{
    public Guid Id { get; set; }

    // Optional: only set when the caller wants idempotency
    public string? IdempotencyKey { get; set; }

    // Optional links (very useful later)
    public Guid? PatientId { get; set; }
    public Guid? EncounterId { get; set; }

    public string Source { get; set; } = "unknown";
    public string EventType { get; set; } = default!;

    // Stored as jsonb in Postgres via EF config
    public string PayloadJson { get; set; } = "{}";

    // Event time (from source)
    public DateTimeOffset OccurredAtUtc { get; set; }

    // Ingest time (server time)
    public DateTimeOffset IngestedAtUtc { get; set; }
}
