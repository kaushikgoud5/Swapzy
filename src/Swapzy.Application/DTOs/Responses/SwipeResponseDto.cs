namespace Swapzy.Application.DTOs.Responses
{
    public class BatchSwipeResponseDto
    {
        public int Processed { get; set; }
        public List<SwipeMatchDto> Matches { get; set; } = new();
    }

    public class SwipeMatchDto
    {
        public int ProductId { get; set; }
        public Guid MatchedWithUserId { get; set; }
    }
}
