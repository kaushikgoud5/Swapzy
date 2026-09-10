using Microsoft.EntityFrameworkCore;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Interests;
using Swapzy.Core.Exceptions;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly SwapzyDbContext _context;

    public ChatService(SwapzyDbContext context) => _context = context;

    public async Task<ChatMessageDto> SendMessageAsync(Guid matchId, Guid senderId, string text)
    {
        var match = await _context.Matches
            .FirstOrDefaultAsync(m => m.Id == matchId && m.DateDeleted == null)
            ?? throw new NotFoundException($"Match {matchId} not found.");

        if (match.BuyerId != senderId && match.SellerId != senderId)
            throw new ForbiddenException("You are not a participant of this match.");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            SenderId = senderId,
            Text = text,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = senderId.ToString()
        };

        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

        return ToDto(message);
    }

    public async Task<List<ChatMessageDto>> GetMessagesAsync(Guid matchId, Guid userId, int page, int pageSize)
    {
        var match = await _context.Matches
            .FirstOrDefaultAsync(m => m.Id == matchId && m.DateDeleted == null)
            ?? throw new NotFoundException($"Match {matchId} not found.");

        if (match.BuyerId != userId && match.SellerId != userId)
            throw new ForbiddenException("You are not a participant of this match.");

        return await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.MatchId == matchId && m.DateDeleted == null)
            .OrderBy(m => m.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(ToDto).ToList());
    }

    private static ChatMessageDto ToDto(Message m) => new()
    {
        Id = m.Id,
        MatchId = m.MatchId,
        SenderId = m.SenderId,
        SenderName = m.Sender.Name,
        Text = m.Text,
        CreatedOn = m.CreatedOn
    };
}
