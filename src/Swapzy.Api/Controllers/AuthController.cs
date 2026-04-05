using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var userId = await authService.RegisterAsync(dto);
        return StatusCode(201, new { userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var response = await authService.LoginAsync(dto);
        return Ok(response);
    }
}
