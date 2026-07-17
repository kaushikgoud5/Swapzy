using Swapzy.Core.Entities.Matches;

namespace Swapzy.Application.DTOs.Responses
{
    public class MatchResponseDto
    {
        public int Id { get; set; }
        public Guid InterestedUserId { get; set; }
        public Guid SellerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public double EstimatedValue { get; set; }
        public MatchStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CancelledAt { get; set; }
    }
}
