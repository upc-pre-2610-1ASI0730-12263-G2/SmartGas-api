using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.DTOs;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/settings")]
public class SettingsController : ControllerBase
{
    private readonly SettingService _settingService;
    private readonly IStringLocalizer<SettingsController> _localizer;

    public SettingsController(SettingService settingService, IStringLocalizer<SettingsController> localizer)
    {
        _settingService = settingService;
        _localizer = localizer;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetByAccount([FromRoute] int accountId)
    {
        var setting = await _settingService.GetByAccountAsync(accountId);

        if (setting == null)
        {
            return NotFound(new
            {
                message = _localizer["SettingsNotFound"].Value
            });
        }

        return Ok(setting);
    }

    [HttpPatch("{accountId}")]
    public async Task<IActionResult> Update([FromRoute] int accountId, [FromBody] UpdateSettingRequest request)
    {
        var setting = await _settingService.UpdateAsync(accountId, request);

        if (setting == null)
        {
            return NotFound(new
            {
                message = _localizer["SettingsNotFound"].Value
            });
        }

        return Ok(setting);
    }
}
