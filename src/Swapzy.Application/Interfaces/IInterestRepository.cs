using Swapzy.Core.Entities.Interests;

namespace Swapzy.Application.Interfaces;

public interface IInterestRepository
{
    Task<Interest?> GetByIdAsync(Guid id);
    Task<Interest?> GetByBuyerAndProductAsync(Guid buyerId, int productId);
    Task<List<Interest>> GetBySellerAsync(Guid sellerId, int page, int pageSize);
    Task<List<Interest>> GetByBuyerAsync(Guid buyerId, int page, int pageSize);
    Task AddAsync(Interest interest);
}
