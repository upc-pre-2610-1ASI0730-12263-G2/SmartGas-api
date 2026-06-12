using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartGas.Api.DTOs;
using SmartGas.Api.Services;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IStringLocalizer<AuthController> _localizer;

    public AuthController(AuthService authService, IStringLocalizer<AuthController> localizer)
    {
        _authService = authService;
        _localizer = localizer;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        var result = await _authService.SignUpAsync(request);

        if (result == null)
        {
            return Conflict(new
            {
                message = _localizer["EmailAlreadyRegisteredOrDefaultPlanUnavailable"].Value
            });
        }

        return Created("/api/v1/auth/sign-up", result);
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var result = await _authService.SignInAsync(request);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = _localizer["InvalidCredentials"].Value
            });
        }

        return Ok(result);
    }
}
