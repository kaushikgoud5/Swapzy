using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Categories;
using Swapzy.Core.Exceptions;

namespace Swapzy.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            IUnitOfWork unitOfWork,
            ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, Guid userId)
        {
            if (await _unitOfWork.Categories.SlugExistsAsync(dto.Slug))
                throw new ConflictException($"Category with slug '{dto.Slug}' already exists.");

            if (dto.ParentCategoryId.HasValue)
            {
                var parentCategory = await _unitOfWork.Categories.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parentCategory == null)
                    throw new NotFoundException($"Parent category with ID {dto.ParentCategoryId} not found.");
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Slug = dto.Slug.ToLower(),
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = true,
                CreatedBy = userId.ToString(),
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<CategoryResponseDto?> GetBySlugAsync(string slug)
        {
            var category = await _unitOfWork.Categories.GetBySlugAsync(slug);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync(bool includeInactive = false)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(includeInactive);
            return categories.Select(MapToDto).ToList();
        }

        public async Task<List<CategoryResponseDto>> GetSubCategoriesAsync(int parentId)
        {
            var categories = await _unitOfWork.Categories.GetSubCategoriesAsync(parentId);
            return categories.Select(MapToDto).ToList();
        }

        public async Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto, Guid userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException($"Category with ID {id} not found.");

            if (!string.IsNullOrEmpty(dto.Slug) && dto.Slug != category.Slug)
            {
                if (await _unitOfWork.Categories.SlugExistsAsync(dto.Slug, id))
                    throw new ConflictException($"Category with slug '{dto.Slug}' already exists.");
                category.Slug = dto.Slug.ToLower();
            }

            if (!string.IsNullOrEmpty(dto.Name))
                category.Name = dto.Name;

            if (dto.Description != null)
                category.Description = dto.Description;

            if (dto.IsActive.HasValue)
                category.IsActive = dto.IsActive.Value;

            if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId != category.ParentCategoryId)
            {
                if (dto.ParentCategoryId == id)
                    throw new BadRequestException("Category cannot be its own parent.");

                var parentCategory = await _unitOfWork.Categories.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parentCategory == null)
                    throw new NotFoundException($"Parent category with ID {dto.ParentCategoryId} not found.");
                category.ParentCategoryId = dto.ParentCategoryId;
            }

            category.ModifiedBy = userId.ToString();
            category.ModifiedOn = DateTime.UtcNow;

            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(int id, Guid userId)
        {
            var productCount = await _unitOfWork.Categories.GetProductCountAsync(id);
            if (productCount > 0)
                throw new ConflictException($"Cannot delete category with {productCount} associated products.");

            var subCategories = await _unitOfWork.Categories.GetSubCategoriesAsync(id);
            if (subCategories.Any())
                throw new ConflictException("Cannot delete category with subcategories.");

            var result = await _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<bool> ToggleActiveStatusAsync(int id, Guid userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException($"Category with ID {id} not found.");

            category.IsActive = !category.IsActive;
            category.ModifiedBy = userId.ToString();
            category.ModifiedOn = DateTime.UtcNow;

            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.IsActive;
        }

        private static CategoryResponseDto MapToDto(Category category)
        {
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description ?? string.Empty,
                Slug = category.Slug,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId,
                CreatedOn = category.CreatedOn,
                ModifiedOn = category.ModifiedOn
            };
        }
    }
}
