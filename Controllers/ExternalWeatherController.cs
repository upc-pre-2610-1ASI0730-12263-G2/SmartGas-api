using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.DTOs;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/external/weather")]
public class ExternalWeatherController : ControllerBase
{
    private readonly ExternalWeatherService _externalWeatherService;
    private readonly IStringLocalizer<ExternalWeatherController> _localizer;

    public ExternalWeatherController(ExternalWeatherService externalWeatherService, IStringLocalizer<ExternalWeatherController> localizer)
    {
        _externalWeatherService = externalWeatherService;
        _localizer = localizer;
    }

    [HttpGet("current")]
    [ProducesResponseType(typeof(ExternalWeatherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetCurrentWeather(
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            return BadRequest(new
            {
                message = _localizer["InvalidLatitude"].Value
            });
        }

        if (longitude < -180 || longitude > 180)
        {
            return BadRequest(new
            {
                message = _localizer["InvalidLongitude"].Value
            });
        }

        try
        {
            var result = await _externalWeatherService.GetCurrentWeatherAsync(latitude, longitude);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = _localizer["ExternalWeatherNoData"].Value
                });
            }

            return Ok(result);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = _localizer["ExternalWeatherUnavailable"].Value
            });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = _localizer["ExternalWeatherTimeout"].Value
            });
        }
        catch (JsonException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = _localizer["ExternalWeatherInvalidResponse"].Value
            });
        }
    }
}
