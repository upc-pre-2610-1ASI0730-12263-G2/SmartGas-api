using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    private readonly IStringLocalizer<DashboardController> _localizer;

    public DashboardController(DashboardService dashboardService, IStringLocalizer<DashboardController> localizer)
    {
        _dashboardService = dashboardService;
        _localizer = localizer;
    }

    [HttpGet("summary/{accountId}")]
    public async Task<IActionResult> GetSummary(int accountId)
    {
        var result = await _dashboardService.GetSummaryAsync(accountId);

        if (result == null)
        {
            return NotFound(new
            {
                message = _localizer["AccountNotFound"].Value
            });
        }

        return Ok(result);
    }
}
