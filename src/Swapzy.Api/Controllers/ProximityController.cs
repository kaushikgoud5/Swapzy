using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("products/nearby")]
[Authorize]
public class ProximityController(IProximityService proximityService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNearbyProducts(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await proximityService.GetNearbyProductsAsync(latitude, longitude, radiusKm, userId, page, pageSize);
        return Ok(new { products = results, page, pageSize, hasMore = results.Count == pageSize });
    }
}
