using System.ComponentModel.DataAnnotations;

namespace Swapzy.Application.DTOs.Requests
{
    public class OnboardingRequestDto
    {
        // Step 1: Avatar
        public string? AvatarUrl { get; set; }

        // Step 2: Bio
        [Required]
        public string DisplayName { get; set; } = default!;

        public string? Bio { get; set; }

        // Step 3: Preferred Categories (min 3)
        [Required]
        [MinLength(3, ErrorMessage = "At least 3 preferred categories are required.")]
        public List<int> PreferredCategoryIds { get; set; } = new();

        // Step 4: Location
        [Required]
        public OnboardingLocationDto Location { get; set; } = default!;
    }

    public class OnboardingLocationDto
    {
        [Required]
        public string Country { get; set; } = default!;

        [Required]
        public string State { get; set; } = default!;

        [Required]
        public string City { get; set; } = default!;

        public string? PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
