using Swapzy.Core.Entities.Products;
using Swapzy.Core.Enums;

namespace Swapzy.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync(Guid? ownerId = null, int? categoryId = null, ProductStatus? status = null, int page = 1, int pageSize = 20);
        Task<Product> UpdateAsync(Product product);
        Task<bool> SoftDeleteAsync(int id);
    }
}
