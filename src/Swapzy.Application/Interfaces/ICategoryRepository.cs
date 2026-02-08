using Swapzy.Core.Entities.Categories;

namespace Swapzy.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> AddAsync(Category category);
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetBySlugAsync(string slug);
        Task<List<Category>> GetAllAsync(bool includeInactive = false);
        Task<List<Category>> GetSubCategoriesAsync(int parentId);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task<int> GetProductCountAsync(int categoryId);
    }
}