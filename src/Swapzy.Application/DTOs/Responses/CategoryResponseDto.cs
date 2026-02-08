namespace Swapzy.Application.DTOs.Responses
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public bool IsActive { get; set; }
        public int? ParentCategoryId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}