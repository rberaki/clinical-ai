using Clinical.Application.Predictions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api/predictions")]
public sealed class PredictionsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("from-feature-run/{featureRunId:guid}")]
    public async Task<ActionResult<CreatePredictionAndAlertResult>> CreateFromFeatureRun(
        Guid featureRunId,
        [FromBody] CreatePredictionAndAlertRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreatePredictionAndAlertCommand(featureRunId, request.HorizonHours, request.Threshold),
            cancellationToken);

        return Ok(result);
    }
}

public sealed record CreatePredictionAndAlertRequest(int HorizonHours, double Threshold);
