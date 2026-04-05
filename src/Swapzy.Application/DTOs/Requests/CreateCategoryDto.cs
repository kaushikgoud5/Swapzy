using System.ComponentModel.DataAnnotations;

namespace Swapzy.Application.DTOs.Requests
{
    public class CreateCategoryDto
    {
        [Required]
        public required string Name { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public required string Slug { get; set; }
        
        public int? ParentCategoryId { get; set; }
    }
}