using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.DTOs;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/emergency-contacts")]
public class EmergencyContactsController : ControllerBase
{
    private readonly EmergencyContactService _emergencyContactService;
    private readonly IStringLocalizer<EmergencyContactsController> _localizer;

    public EmergencyContactsController(EmergencyContactService emergencyContactService, IStringLocalizer<EmergencyContactsController> localizer)
    {
        _emergencyContactService = emergencyContactService;
        _localizer = localizer;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetByAccount(int accountId)
    {
        var result = await _emergencyContactService.GetByAccountAsync(accountId);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["AccountNotFound"].Value
            });
        }

        return Ok(result);
    }

    [HttpPatch("{accountId}")]
    public async Task<IActionResult> Update(
        int accountId,
        [FromBody] UpdateEmergencyContactRequest request)
    {
        try
        {
            var result = await _emergencyContactService.UpdateAsync(accountId, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = _localizer["AccountNotFound"].Value
                });
            }

            return Ok(result);
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
        return exception.Message switch
        {
            "Emergency contact name is required." => _localizer["EmergencyContactNameRequired"].Value,
            "Emergency contact phone is required." => _localizer["EmergencyContactPhoneRequired"].Value,
            "Emergency contact email is required." => _localizer["EmergencyContactEmailRequired"].Value,
            _ => exception.Message
        };
    }
}
