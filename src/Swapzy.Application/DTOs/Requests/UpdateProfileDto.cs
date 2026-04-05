using Swapzy.Application.DTOs.Requests;

namespace Swapzy.Application.DTOs.Requests
{
    public class UpdateProfileDto
    {
        public string? AvatarUrl { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public OnboardingLocationDto? Location { get; set; }
        public List<int>? PreferredCategoryIds { get; set; }
    }
}
