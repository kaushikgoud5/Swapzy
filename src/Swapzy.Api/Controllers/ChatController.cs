using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("chat")]
[Authorize]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpPost("{matchId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid matchId, [FromBody] SendMessageRequest dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await chatService.SendMessageAsync(matchId, userId, dto.Text);
        return StatusCode(201, new { message = result });
    }

    [HttpGet("{matchId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid matchId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await chatService.GetMessagesAsync(matchId, userId, page, pageSize);
        return Ok(new { messages = results, page, pageSize, hasMore = results.Count == pageSize });
    }
}

public record SendMessageRequest(string Text);
