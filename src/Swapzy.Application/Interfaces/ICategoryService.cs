using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, Guid userId);
        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<CategoryResponseDto?> GetBySlugAsync(string slug);
        Task<List<CategoryResponseDto>> GetAllAsync(bool includeInactive = false);
        Task<List<CategoryResponseDto>> GetSubCategoriesAsync(int parentId);
        Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto, Guid userId);
        Task<bool> DeleteAsync(int id, Guid userId);
        Task<bool> ToggleActiveStatusAsync(int id, Guid userId);
    }
}