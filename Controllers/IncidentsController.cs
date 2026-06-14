using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly IncidentService _incidentService;
    private readonly IStringLocalizer<IncidentsController> _localizer;

    public IncidentsController(IncidentService incidentService, IStringLocalizer<IncidentsController> localizer)
    {
        _incidentService = incidentService;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetByAccount([FromQuery] int accountId)
    {
        var incidents = await _incidentService.GetByAccountAsync(accountId);
        return Ok(incidents);
    }

    [HttpPatch("{id}/review")]
    public async Task<IActionResult> Review([FromRoute] int id)
    {
        var result = await _incidentService.ReviewAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["IncidentNotFound"].Value
            });
        }

        return Ok(result);
    }

    [HttpPatch("{id}/resolve")]
    public async Task<IActionResult> Resolve([FromRoute] int id)
    {
        var result = await _incidentService.ResolveAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["IncidentNotFound"].Value
            });
        }

        return Ok(result);
    }

    [HttpPatch("{id}/false-alarm")]
    public async Task<IActionResult> MarkAsFalseAlarm([FromRoute] int id)
    {
        var result = await _incidentService.MarkAsFalseAlarmAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["IncidentNotFound"].Value
            });
        }

        return Ok(result);
    }
}
