using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("matches")]
[Authorize]
public class MatchesController(IMatchService matchService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyMatches([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await matchService.GetMyMatchesAsync(userId, page, pageSize);
        return Ok(new { matches = results, page, pageSize, hasMore = results.Count == pageSize });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await matchService.GetByIdAsync(id, userId);
        return Ok(new { match = result });
    }

    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await matchService.CancelAsync(id, userId);
        return Ok(new { match = result });
    }
}
