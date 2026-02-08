using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Categories;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SwapzyDbContext _context;

        public CategoryRepository(SwapzyDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.DateDeleted == null);
        }

        public async Task<Category?> GetBySlugAsync(string slug)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug && c.DateDeleted == null);
        }

        public async Task<List<Category>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Categories
                .Where(c => c.DateDeleted == null);

            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            return await query
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> GetSubCategoriesAsync(int parentId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentId && c.DateDeleted == null && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null) return false;

            category.DateDeleted = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Categories
                .Where(c => c.Slug == slug && c.DateDeleted == null);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetProductCountAsync(int categoryId)
        {
            return await _context.Products
                .CountAsync(p => p.ProductCategoryId == categoryId && p.DateDeleted == null);
        }
    }
}