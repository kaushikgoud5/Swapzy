namespace Swapzy.Application.DTOs.Requests
{
    public class UpdateCategoryDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public bool? IsActive { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}