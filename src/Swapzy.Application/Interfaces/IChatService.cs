using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces;

public interface IChatService
{
    Task<ChatMessageDto> SendMessageAsync(Guid matchId, Guid senderId, string text);
    Task<List<ChatMessageDto>> GetMessagesAsync(Guid matchId, Guid userId, int page, int pageSize);
}
