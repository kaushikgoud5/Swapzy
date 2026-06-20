using Swapzy.Core.Enums;

namespace Swapzy.Application.DTOs.Responses
{
    public class NearbyProductResponseDto
    {
        public int Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string Condition { get; set; } = default!;
        public int ProductCategoryId { get; set; }
        public double EstimatedValue { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsAvailable { get; set; }
        public ProductLocationResponseDto? Location { get; set; }
        public double DistanceKm { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
