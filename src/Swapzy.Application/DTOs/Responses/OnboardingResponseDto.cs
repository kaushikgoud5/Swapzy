namespace Swapzy.Application.DTOs.Responses
{
    public class OnboardingResponseDto
    {
        public Guid UserId { get; set; }
        public bool IsOnboarded { get; set; }
        public string? AvatarUrl { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public OnboardingLocationResponseDto? Location { get; set; }
        public List<int> PreferredCategoryIds { get; set; } = new();
    }

    public class OnboardingLocationResponseDto
    {
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string City { get; set; } = default!;
        public string? PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
