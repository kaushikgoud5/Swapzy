namespace Swapzy.Application.DTOs.Responses;

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = default!;
    public string Text { get; set; } = default!;
    public DateTime CreatedOn { get; set; }
}
