using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Enums;
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

        public async Task<List<Product>> GetAllAsync(Guid? ownerId = null, int? categoryId = null, ProductStatus? status = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Products
                .Include(p => p.Location)
                .Where(p => p.DateDeleted == null);

            if (ownerId.HasValue)
                query = query.Where(p => p.OwnerId == ownerId.Value);

            if (categoryId.HasValue)
                query = query.Where(p => p.ProductCategoryId == categoryId.Value);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            return await query
                .OrderByDescending(p => p.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.DateDeleted == null);
            if (product == null) return false;

            product.DateDeleted = DateTime.UtcNow;
            product.IsActive = false;
            product.IsAvailable = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
