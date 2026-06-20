using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface IProximityService
    {
        Task<List<NearbyProductResponseDto>> GetNearbyProductsAsync(double latitude, double longitude, double radiusKm, Guid? excludeUserId = null, int page = 1, int pageSize = 20);
    }
}
