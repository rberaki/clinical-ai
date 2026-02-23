using Clinical.Application.Alerts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class AlertsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("alerts/{alertId:guid}/acknowledge")]
    public async Task<ActionResult<AcknowledgeAlertResult>> Acknowledge(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new AcknowledgeAlertCommand(alertId), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("encounters/{encounterId:guid}/alerts")]
    public async Task<ActionResult<IReadOnlyList<EncounterAlertDto>>> GetEncounterAlerts(
        Guid encounterId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEncounterAlertsQuery(encounterId, status), cancellationToken);
        return Ok(result);
    }
}
