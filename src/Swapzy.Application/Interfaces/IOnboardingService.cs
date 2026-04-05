using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface IOnboardingService
    {
        Task<OnboardingResponseDto> CompleteOnboardingAsync(OnboardingRequestDto dto, Guid userId);
        Task<OnboardingResponseDto> GetOnboardingStatusAsync(Guid userId);
    }
}
