using Microsoft.EntityFrameworkCore;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Exceptions;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly SwapzyDbContext _context;
        private readonly IStorageService _storageService;

        public ProductImageService(SwapzyDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<UploadUrlResponseDto> GenerateUploadUrlAsync(int productId, string contentType, Guid userId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.DateDeleted == null)
                ?? throw new NotFoundException("Product not found.");

            if (product.OwnerId != userId)
                throw new ForbiddenException("Not your product.");

            var extension = contentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".jpg"
            };

            var key = $"products/{productId}/{Guid.NewGuid()}{extension}";
            var uploadUrl = await _storageService.GenerateUploadUrlAsync(key, contentType);

            return new UploadUrlResponseDto
            {
                UploadUrl = uploadUrl,
                S3Key = key
            };
        }

        public async Task<ProductImageResponseDto> ConfirmUploadAsync(int productId, string s3Key, string contentType, Guid userId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.DateDeleted == null)
                ?? throw new NotFoundException("Product not found.");

            if (product.OwnerId != userId)
                throw new ForbiddenException("Not your product.");

            var maxOrder = await _context.ProductImages
                .Where(i => i.ProductId == productId && i.DateDeleted == null)
                .MaxAsync(i => (int?)i.DisplayOrder) ?? 0;

            var image = new ProductImage
            {
                ProductId = productId,
                S3Key = s3Key,
                ContentType = contentType,
                DisplayOrder = maxOrder + 1,
                CreatedBy = userId.ToString(),
                CreatedOn = DateTime.UtcNow
            };

            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();

            var url = await _storageService.GenerateReadUrlAsync(image.S3Key);

            return new ProductImageResponseDto
            {
                Id = image.Id,
                Url = url,
                DisplayOrder = image.DisplayOrder
            };
        }

        public async Task<List<ProductImageResponseDto>> GetAllAsync(int productId)
        {
            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId && i.DateDeleted == null)
                .OrderBy(i => i.DisplayOrder)
                .ToListAsync();

            var result = new List<ProductImageResponseDto>();
            foreach (var image in images)
            {
                var url = await _storageService.GenerateReadUrlAsync(image.S3Key);
                result.Add(new ProductImageResponseDto
                {
                    Id = image.Id,
                    Url = url,
                    DisplayOrder = image.DisplayOrder
                });
            }

            return result;
        }

        public async Task DeleteAsync(int productId, int imageId, Guid userId)
        {
            var image = await _context.ProductImages
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == productId && i.DateDeleted == null)
                ?? throw new NotFoundException("Image not found.");

            if (image.Product.OwnerId != userId)
                throw new ForbiddenException("Not your product.");

            await _storageService.DeleteAsync(image.S3Key);
            image.DateDeleted = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
