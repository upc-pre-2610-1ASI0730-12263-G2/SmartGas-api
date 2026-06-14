using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notificationService;
    private readonly IStringLocalizer<NotificationsController> _localizer;

    public NotificationsController(NotificationService notificationService, IStringLocalizer<NotificationsController> localizer)
    {
        _notificationService = notificationService;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetByAccount([FromQuery] int accountId)
    {
        var notifications = await _notificationService.GetByAccountAsync(accountId);
        return Ok(notifications);
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead([FromRoute] int id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["NotificationNotFound"].Value
            });
        }

        return Ok(result);
    }

    [HttpPatch("{id}/confirm")]
    public async Task<IActionResult> Confirm([FromRoute] int id)
    {
        var result = await _notificationService.ConfirmAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["NotificationNotFound"].Value
            });
        }

        return Ok(result);
    }
}
