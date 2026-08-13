using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Enums;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("interests")]
[Authorize]
public class InterestsController(IInterestService interestService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ExpressInterest([FromBody] InterestRequestDto dto)
    {
        var buyerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await interestService.ExpressInterestAsync(buyerId, dto.ProductId);
        return StatusCode(201, new { interest = result });
    }

    [HttpGet("seller")]
    public async Task<IActionResult> GetForSeller([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await interestService.GetInterestsForSellerAsync(sellerId, page, pageSize);
        return Ok(new { interests = results, page, pageSize, hasMore = results.Count == pageSize });
    }

    [HttpGet("buyer")]
    public async Task<IActionResult> GetForBuyer([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var buyerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await interestService.GetInterestsForBuyerAsync(buyerId, page, pageSize);
        return Ok(new { interests = results, page, pageSize, hasMore = results.Count == pageSize });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] InterestStatus status)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await interestService.UpdateStatusAsync(id, status, userId);
        return Ok(new { interest = result });
    }
}
