using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("onboarding")]
[Authorize]
public class OnboardingController(IOnboardingService onboardingService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Complete([FromBody] OnboardingRequestDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await onboardingService.CompleteOnboardingAsync(dto, userId);
        return StatusCode(201, new { onboarding = result });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await onboardingService.GetOnboardingStatusAsync(userId);
        return Ok(new { onboarding = result });
    }
}
