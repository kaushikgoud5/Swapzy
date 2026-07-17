using Swapzy.Core.Enums;

namespace Swapzy.Application.DTOs.Requests
{
    public class SwipeItemDto
    {
        public int ProductId { get; set; }
        public SwipeDirection Direction { get; set; }
    }

    public class BatchSwipeRequestDto
    {
        public List<SwipeItemDto> Swipes { get; set; } = new();
    }
}
