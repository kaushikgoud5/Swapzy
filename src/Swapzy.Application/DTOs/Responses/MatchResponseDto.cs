namespace Swapzy.Application.DTOs.Responses;

public class MatchResponseDto
{
    public Guid Id { get; set; }
    public Guid InterestId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public string? ProductImageUrl { get; set; }
    public bool IsSwapped { get; set; }
    public DateTime CreatedOn { get; set; }
}
