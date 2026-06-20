using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("products/nearby")]
[Authorize]
public class ProximityController(IProximityService proximityService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNearbyProducts([FromQuery] NearbyProductsRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await proximityService.GetNearbyProductsAsync(
            request.Latitude, request.Longitude, request.RadiusKm, userId, request.Page, request.PageSize);
        return Ok(new { products = results });
    }
}
