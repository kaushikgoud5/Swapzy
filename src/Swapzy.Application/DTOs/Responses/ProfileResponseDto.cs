namespace Swapzy.Application.DTOs.Responses
{
    public class ProfileResponseDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public OnboardingLocationResponseDto? Location { get; set; }
        public List<int> PreferredCategoryIds { get; set; } = new();
    }
}
