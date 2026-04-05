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
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IRoleRepository roleRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, Guid userId)
        {
            var permissions = await _roleRepository.GetUserPermissionsAsync(userId);
            if (!permissions.Contains("CREATE_PRODUCT"))
            {
                throw new ForbiddenException("You do not have permission to create products.");
            }

            var category = await _categoryRepository.GetByIdAsync(dto.ProductCategoryId);
            if (category == null || !category.IsActive)
            {
                throw new NotFoundException($"Category with ID {dto.ProductCategoryId} not found or inactive.");
            }

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

            var created = await _productRepository.AddAsync(product);
            _logger.LogInformation("Product created with ID: {ProductId} by user: {UserId}", created.Id, userId);

            return MapToDto(created);
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
