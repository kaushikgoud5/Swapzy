namespace Swapzy.Application.DTOs.Requests
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public int? ProductCategoryId { get; set; }
        public double? EstimatedValue { get; set; }
        public double? OriginalValue { get; set; }
        public CreateProductLocationDto? Location { get; set; }
    }
}
