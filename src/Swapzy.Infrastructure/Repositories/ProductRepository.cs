using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Products;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SwapzyDbContext _context;

        public ProductRepository(SwapzyDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.Id == id && p.DateDeleted == null);
        }
    }
}
