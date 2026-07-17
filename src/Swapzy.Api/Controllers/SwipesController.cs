using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Authorize]
public class SwipesController(ISwipeService swipeService) : ControllerBase
{
    [HttpPost("swipes/batch")]
    public async Task<IActionResult> BatchSwipe([FromBody] BatchSwipeRequestDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await swipeService.BatchSwipeAsync(dto, userId);
        return Ok(result);
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await swipeService.GetFeedAsync(latitude, longitude, radiusKm, userId, page, pageSize);
        return Ok(new { products = results, page, pageSize, hasMore = results.Count == pageSize });
    }
}
