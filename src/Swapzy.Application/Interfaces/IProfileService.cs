using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileResponseDto> GetProfileAsync(Guid userId);
        Task<ProfileResponseDto> UpdateProfileAsync(UpdateProfileDto dto, Guid userId);
    }
}
