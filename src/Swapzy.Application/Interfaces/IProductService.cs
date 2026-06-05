using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Core.Enums;

namespace Swapzy.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto, Guid userId);
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<List<ProductResponseDto>> GetAllAsync(Guid? ownerId = null, int? categoryId = null, ProductStatus? status = null, int page = 1, int pageSize = 20);
        Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto, Guid userId);
        Task<bool> DeleteAsync(int id, Guid userId);
        Task<ProductResponseDto> ToggleAvailabilityAsync(int id, Guid userId);
    }
}
