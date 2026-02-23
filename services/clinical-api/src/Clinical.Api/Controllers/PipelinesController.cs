using Clinical.Application.Pipelines;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api/pipelines")]
public sealed class PipelinesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("features_hourly/run")]
    public async Task<ActionResult<RunHourlyFeaturesPipelineResult>> RunHourlyFeaturesPipeline(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RunHourlyFeaturesPipelineCommand(), cancellationToken);
        return Ok(result);
    }
}
