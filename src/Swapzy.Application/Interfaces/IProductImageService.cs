using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<UploadUrlResponseDto> GenerateUploadUrlAsync(int productId, string contentType, Guid userId);
        Task<ProductImageResponseDto> ConfirmUploadAsync(int productId, string s3Key, string contentType, Guid userId);
        Task<List<ProductImageResponseDto>> GetAllAsync(int productId);
        Task DeleteAsync(int productId, int imageId, Guid userId);
    }
}
