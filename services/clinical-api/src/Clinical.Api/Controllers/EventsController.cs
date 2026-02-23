using Clinical.Application.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<IngestRawEventResult>> Post(
        [FromBody] IngestRawEventRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new IngestRawEventCommand(
                request.IdempotencyKey,
                request.PatientId,
                request.EncounterId,
                request.SourceSystem,
                request.EventType,
                request.PayloadJson,
                request.OccurredAtUtc),
            cancellationToken);

        if (result.IsDuplicate)
        {
            return Ok(result);
        }

        return Created($"/api/events/{result.RawEventId}", result);
    }
}

public sealed record IngestRawEventRequest(
    string? IdempotencyKey,
    Guid? PatientId,
    Guid? EncounterId,
    string SourceSystem,
    string EventType,
    string PayloadJson,
    DateTimeOffset OccurredAtUtc);
