using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.DTOs;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly ProfileService _profileService;
    private readonly IStringLocalizer<ProfilesController> _localizer;

    public ProfilesController(ProfileService profileService, IStringLocalizer<ProfilesController> localizer)
    {
        _profileService = profileService;
        _localizer = localizer;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetByAccount([FromRoute] int accountId)
    {
        var profile = await _profileService.GetByAccountAsync(accountId);

        if (profile == null)
        {
            return NotFound(new
            {
                message = _localizer["ProfileNotFound"].Value
            });
        }

        return Ok(profile);
    }

    [HttpPatch("{accountId}")]
    public async Task<IActionResult> Update([FromRoute] int accountId, [FromBody] UpdateProfileRequest request)
    {
        var profile = await _profileService.UpdateAsync(accountId, request);

        if (profile == null)
        {
            return NotFound(new
            {
                message = _localizer["ProfileNotFound"].Value
            });
        }

        return Ok(profile);
    }
}
