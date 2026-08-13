using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Interests;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Repositories;

public class InterestRepository : IInterestRepository
{
    private readonly SwapzyDbContext _context;

    public InterestRepository(SwapzyDbContext context) => _context = context;

    public Task<Interest?> GetByIdAsync(Guid id) =>
        _context.Interests.Include(i => i.Product).FirstOrDefaultAsync(i => i.Id == id);

    public Task<Interest?> GetByBuyerAndProductAsync(Guid buyerId, int productId) =>
        _context.Interests.FirstOrDefaultAsync(i => i.BuyerId == buyerId && i.ProductId == productId);

    public Task<List<Interest>> GetBySellerAsync(Guid sellerId, int page, int pageSize) =>
        _context.Interests
            .Include(i => i.Product)
            .Where(i => i.SellerId == sellerId && i.DateDeleted == null)
            .OrderByDescending(i => i.CreatedOn)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

    public Task<List<Interest>> GetByBuyerAsync(Guid buyerId, int page, int pageSize) =>
        _context.Interests
            .Include(i => i.Product)
            .Where(i => i.BuyerId == buyerId && i.DateDeleted == null)
            .OrderByDescending(i => i.CreatedOn)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

    public async Task AddAsync(Interest interest) =>
        await _context.Interests.AddAsync(interest);
}
