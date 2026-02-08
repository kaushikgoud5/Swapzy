using System.ComponentModel.DataAnnotations;

namespace Swapzy.Application.DTOs.Requests
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public string Slug { get; set; }
        
        public int? ParentCategoryId { get; set; }
    }
}