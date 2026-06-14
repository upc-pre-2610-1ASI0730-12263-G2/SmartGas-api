using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.DTOs;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/sensor-readings")]
public class SensorReadingsController : ControllerBase
{
    private readonly SensorReadingService _sensorReadingService;
    private readonly IStringLocalizer<SensorReadingsController> _localizer;

    public SensorReadingsController(SensorReadingService sensorReadingService, IStringLocalizer<SensorReadingsController> localizer)
    {
        _sensorReadingService = sensorReadingService;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetByAccount([FromQuery] int accountId)
    {
        var readings = await _sensorReadingService.GetByAccountAsync(accountId);
        return Ok(readings);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSensorReadingRequest request)
    {
        var result = await _sensorReadingService.CreateAsync(request);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["SensorNotFound"].Value
            });
        }

        return Created($"/api/v1/sensor-readings/{result.Id}", result);
    }
}
