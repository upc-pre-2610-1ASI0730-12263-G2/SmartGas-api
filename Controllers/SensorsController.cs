using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.DTOs;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/sensors")]
public class SensorsController : ControllerBase
{
    private readonly SensorService _sensorService;
    private readonly IStringLocalizer<SensorsController> _localizer;

    public SensorsController(SensorService sensorService, IStringLocalizer<SensorsController> localizer)
    {
        _sensorService = sensorService;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetByAccount([FromQuery] int accountId)
    {
        var sensors = await _sensorService.GetByAccountAsync(accountId);
        return Ok(sensors);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSensorRequest request)
    {
        try
        {
            var result = await _sensorService.CreateAsync(request);

            if (result == null)
            {
                return BadRequest(new
                {
                    message = _localizer["InvalidAccountZoneOrSensorCode"].Value
                });
            }

            return Created($"/api/v1/sensors/{result.Id}", result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = LocalizeException(exception)
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = LocalizeException(exception)
            });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateSensorRequest request)
    {
        try
        {
            var result = await _sensorService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = _localizer["SensorNotFound"].Value
                });
            }

            return Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = LocalizeException(exception)
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = LocalizeException(exception)
            });
        }
    }

    private string LocalizeException(Exception exception)
    {
        var message = exception.Message;

        if (message.StartsWith("Plan limit reached. Your current plan allows up to") && message.EndsWith("sensors."))
        {
            var maxSensors = message
                .Replace("Plan limit reached. Your current plan allows up to", string.Empty)
                .Replace("sensors.", string.Empty)
                .Trim();

            return _localizer["SensorPlanLimitReached", maxSensors].Value;
        }

        return message switch
        {
            "A valid account id is required." => _localizer["ValidAccountIdRequired"].Value,
            "A valid zone id is required." => _localizer["ValidZoneIdRequired"].Value,
            "A sensor code is required." => _localizer["SensorCodeRequired"].Value,
            "A sensor name is required." => _localizer["SensorNameRequired"].Value,
            "The selected zone does not belong to this account." => _localizer["SelectedZoneDoesNotBelongToAccount"].Value,
            "A sensor with the same code already exists." => _localizer["SensorCodeAlreadyExists"].Value,
            "The sensor status is invalid." => _localizer["InvalidSensorStatus"].Value,
            "The account does not have an active subscription." => _localizer["NoActiveSubscription"].Value,
            _ => message
        };
    }
}
