using Swapzy.Core.Entities.Products;

namespace Swapzy.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<Product?> GetByIdAsync(int id);
    }
}
