using System.ComponentModel.DataAnnotations;

namespace Swapzy.Application.DTOs.Requests
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [Required]
        public string Condition { get; set; } = default!;

        [Required]
        public int ProductCategoryId { get; set; }

        [Range(0, double.MaxValue)]
        public double EstimatedValue { get; set; }

        [Range(0, double.MaxValue)]
        public double OriginalValue { get; set; }

        public CreateProductLocationDto? Location { get; set; }
    }

    public class CreateProductLocationDto
    {
        [Required]
        public string Country { get; set; } = default!;

        [Required]
        public string State { get; set; } = default!;

        [Required]
        public string City { get; set; } = default!;

        [Required]
        public string PostalCode { get; set; } = default!;

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
