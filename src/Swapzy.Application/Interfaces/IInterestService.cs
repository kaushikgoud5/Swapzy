using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Core.Enums;

namespace Swapzy.Application.Interfaces;

public interface IInterestService
{
    Task<InterestResponseDto> ExpressInterestAsync(Guid buyerId, int productId);
    Task<List<InterestResponseDto>> GetInterestsForSellerAsync(Guid sellerId, int page = 1, int pageSize = 20);
    Task<List<InterestResponseDto>> GetInterestsForBuyerAsync(Guid buyerId, int page = 1, int pageSize = 20);
    Task<InterestResponseDto> UpdateStatusAsync(Guid interestId, InterestStatus status, Guid userId);
    Task<List<MatchResponseDto>> GetMatchesAsync(Guid userId, int page, int pageSize);
}
