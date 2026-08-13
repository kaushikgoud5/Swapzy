using Swapzy.Core.Enums;

namespace Swapzy.Application.DTOs.Responses;

public class InterestResponseDto
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = default!;
    public InterestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
