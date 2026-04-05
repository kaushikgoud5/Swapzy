using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("profile")]
[Authorize]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await profileService.GetProfileAsync(userId);
        return Ok(new { profile = result });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfileDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await profileService.UpdateProfileAsync(dto, userId);
        return Ok(new { profile = result });
    }
}
