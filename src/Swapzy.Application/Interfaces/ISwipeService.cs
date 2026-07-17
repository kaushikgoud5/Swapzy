using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface ISwipeService
    {
        Task<BatchSwipeResponseDto> BatchSwipeAsync(BatchSwipeRequestDto dto, Guid swiperId);
        Task<List<NearbyProductResponseDto>> GetFeedAsync(double latitude, double longitude, double radiusKm, Guid userId, int page = 1, int pageSize = 20);
    }
}
