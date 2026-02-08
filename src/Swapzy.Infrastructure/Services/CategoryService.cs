using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Categories;

namespace Swapzy.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, Guid userId)
        {
            _logger.LogInformation("Creating category with slug: {Slug}", dto.Slug);

            if (await _categoryRepository.SlugExistsAsync(dto.Slug))
            {
                throw new InvalidOperationException($"Category with slug '{dto.Slug}' already exists.");
            }

            if (dto.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new InvalidOperationException($"Parent category with ID {dto.ParentCategoryId} not found.");
                }
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

            var created = await _categoryRepository.AddAsync(category);
            _logger.LogInformation("Category created with ID: {CategoryId}", created.Id);

            return MapToDto(created);
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<CategoryResponseDto?> GetBySlugAsync(string slug)
        {
            var category = await _categoryRepository.GetBySlugAsync(slug);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync(bool includeInactive = false)
        {
            var categories = await _categoryRepository.GetAllAsync(includeInactive);
            return categories.Select(MapToDto).ToList();
        }

        public async Task<List<CategoryResponseDto>> GetSubCategoriesAsync(int parentId)
        {
            var categories = await _categoryRepository.GetSubCategoriesAsync(parentId);
            return categories.Select(MapToDto).ToList();
        }

        public async Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto, Guid userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {id} not found.");
            }

            if (!string.IsNullOrEmpty(dto.Slug) && dto.Slug != category.Slug)
            {
                if (await _categoryRepository.SlugExistsAsync(dto.Slug, id))
                {
                    throw new InvalidOperationException($"Category with slug '{dto.Slug}' already exists.");
                }
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
                {
                    throw new InvalidOperationException("Category cannot be its own parent.");
                }

                var parentCategory = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new InvalidOperationException($"Parent category with ID {dto.ParentCategoryId} not found.");
                }
                category.ParentCategoryId = dto.ParentCategoryId;
            }

            category.ModifiedBy = userId.ToString();
            category.ModifiedOn = DateTime.UtcNow;

            var updated = await _categoryRepository.UpdateAsync(category);
            _logger.LogInformation("Category updated: {CategoryId}", id);

            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id, Guid userId)
        {
            var productCount = await _categoryRepository.GetProductCountAsync(id);
            if (productCount > 0)
            {
                throw new InvalidOperationException($"Cannot delete category with {productCount} associated products.");
            }

            var subCategories = await _categoryRepository.GetSubCategoriesAsync(id);
            if (subCategories.Any())
            {
                throw new InvalidOperationException("Cannot delete category with subcategories.");
            }

            _logger.LogInformation("Deleting category: {CategoryId} by user: {UserId}", id, userId);
            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleActiveStatusAsync(int id, Guid userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {id} not found.");
            }

            category.IsActive = !category.IsActive;
            category.ModifiedBy = userId.ToString();
            category.ModifiedOn = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);
            _logger.LogInformation("Category {CategoryId} active status toggled to: {IsActive}", id, category.IsActive);

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