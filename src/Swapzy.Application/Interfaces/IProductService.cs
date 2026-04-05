using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto, Guid userId);
    }
}
