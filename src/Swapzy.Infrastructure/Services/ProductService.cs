using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Enums;
using Swapzy.Core.Exceptions;

namespace Swapzy.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork unitOfWork,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, Guid userId)
        {
            var permissions = await _unitOfWork.Roles.GetUserPermissionsAsync(userId);
            if (!permissions.Contains("CREATE_PRODUCT"))
                throw new ForbiddenException("You do not have permission to create products.");

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.ProductCategoryId);
            if (category == null || !category.IsActive)
                throw new NotFoundException($"Category with ID {dto.ProductCategoryId} not found or inactive.");

            var product = new Product
            {
                OwnerId = userId,
                ProductCategoryId = dto.ProductCategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Condition = dto.Condition,
                EstimatedValue = dto.EstimatedValue,
                OriginalValue = dto.OriginalValue,
                Status = ProductStatus.Available,
                IsAvailable = true,
                IsActive = true,
                CreatedBy = userId.ToString(),
                CreatedOn = DateTime.UtcNow
            };

            if (dto.Location != null)
            {
                product.Location = new ProductLocation
                {
                    Country = dto.Location.Country,
                    State = dto.Location.State,
                    City = dto.Location.City,
                    PostalCode = dto.Location.PostalCode,
                    Latitude = dto.Location.Latitude,
                    Longitude = dto.Location.Longitude,
                    CreatedBy = userId.ToString(),
                    CreatedOn = DateTime.UtcNow
                };
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product created with ID: {ProductId} by user: {UserId}", product.Id, userId);
            return MapToDto(product);
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found.");
            return MapToDto(product);
        }

        public async Task<List<ProductResponseDto>> GetAllAsync(Guid? ownerId = null, int? categoryId = null, ProductStatus? status = null, int page = 1, int pageSize = 20)
        {
            var products = await _unitOfWork.Products.GetAllAsync(ownerId, categoryId, status, page, pageSize);
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto, Guid userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found.");

            if (product.OwnerId != userId)
                throw new ForbiddenException("You can only update your own products.");

            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;

            if (dto.Description != null)
                product.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Condition))
                product.Condition = dto.Condition;

            if (dto.ProductCategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(dto.ProductCategoryId.Value);
                if (category == null || !category.IsActive)
                    throw new NotFoundException($"Category with ID {dto.ProductCategoryId} not found or inactive.");
                product.ProductCategoryId = dto.ProductCategoryId.Value;
            }

            if (dto.EstimatedValue.HasValue)
                product.EstimatedValue = dto.EstimatedValue.Value;

            if (dto.OriginalValue.HasValue)
                product.OriginalValue = dto.OriginalValue.Value;

            if (dto.Location != null)
            {
                if (product.Location == null)
                {
                    product.Location = new ProductLocation
                    {
                        Country = dto.Location.Country,
                        State = dto.Location.State,
                        City = dto.Location.City,
                        PostalCode = dto.Location.PostalCode,
                        Latitude = dto.Location.Latitude,
                        Longitude = dto.Location.Longitude,
                        CreatedBy = userId.ToString(),
                        CreatedOn = DateTime.UtcNow
                    };
                }
                else
                {
                    product.Location.Country = dto.Location.Country;
                    product.Location.State = dto.Location.State;
                    product.Location.City = dto.Location.City;
                    product.Location.PostalCode = dto.Location.PostalCode;
                    product.Location.Latitude = dto.Location.Latitude;
                    product.Location.Longitude = dto.Location.Longitude;
                    product.Location.ModifiedBy = userId.ToString();
                    product.Location.ModifiedOn = DateTime.UtcNow;
                }
            }

            product.ModifiedBy = userId.ToString();
            product.ModifiedOn = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} updated by user: {UserId}", id, userId);
            return MapToDto(product);
        }

        public async Task<bool> DeleteAsync(int id, Guid userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found.");

            if (product.OwnerId != userId)
                throw new ForbiddenException("You can only delete your own products.");

            var result = await _unitOfWork.Products.SoftDeleteAsync(id);
            _logger.LogInformation("Product {ProductId} deleted by user: {UserId}", id, userId);
            return result;
        }

        public async Task<ProductResponseDto> ToggleAvailabilityAsync(int id, Guid userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found.");

            if (product.OwnerId != userId)
                throw new ForbiddenException("You can only modify your own products.");

            product.IsAvailable = !product.IsAvailable;
            product.ModifiedBy = userId.ToString();
            product.ModifiedOn = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} availability toggled to {IsAvailable}", id, product.IsAvailable);
            return MapToDto(product);
        }

        private static ProductResponseDto MapToDto(Product product)
        {
            var dto = new ProductResponseDto
            {
                Id = product.Id,
                OwnerId = product.OwnerId,
                Name = product.Name,
                Description = product.Description,
                Condition = product.Condition,
                ProductCategoryId = product.ProductCategoryId,
                EstimatedValue = product.EstimatedValue,
                OriginalValue = product.OriginalValue,
                Status = product.Status,
                IsAvailable = product.IsAvailable,
                IsActive = product.IsActive,
                CreatedOn = product.CreatedOn,
                ModifiedOn = product.ModifiedOn
            };

            if (product.Location != null)
            {
                dto.Location = new ProductLocationResponseDto
                {
                    Country = product.Location.Country,
                    State = product.Location.State,
                    City = product.Location.City,
                    PostalCode = product.Location.PostalCode,
                    Latitude = product.Location.Latitude,
                    Longitude = product.Location.Longitude
                };
            }

            return dto;
        }
    }
}
