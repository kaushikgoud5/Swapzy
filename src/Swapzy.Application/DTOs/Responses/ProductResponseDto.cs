using Swapzy.Core.Enums;

namespace Swapzy.Application.DTOs.Responses
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string Condition { get; set; } = default!;
        public int ProductCategoryId { get; set; }
        public double EstimatedValue { get; set; }
        public double OriginalValue { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public ProductLocationResponseDto? Location { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class ProductLocationResponseDto
    {
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string City { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
