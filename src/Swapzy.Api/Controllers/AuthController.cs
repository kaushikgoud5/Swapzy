using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

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

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] SocialLoginRequestDto dto)
    {
        var response = await authService.SocialLoginAsync("google", dto.Token);
        return Ok(response);
    }

    [HttpPost("github")]
    public async Task<IActionResult> GitHubLogin([FromBody] SocialLoginRequestDto dto)
    {
        var response = await authService.SocialLoginAsync("github", dto.Token);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var response = await authService.RefreshTokenAsync(dto.RefreshToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await authService.LogoutAsync(userId, dto.RefreshToken);
        return Ok(new { message = "Logged out successfully." });
    }

    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await authService.RevokeAllTokensAsync(userId);
        return Ok(new { message = "All tokens revoked successfully." });
    }
}
