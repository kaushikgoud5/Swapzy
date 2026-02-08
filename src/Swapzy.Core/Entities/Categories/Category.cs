using Swapzy.Core.Common;
using Swapzy.Core.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Categories
{
    public class Category :BaseAuditableEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Slug { get; set; }
        public bool IsActive { get; set; } = true;
        public string? IconUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        public ICollection<UserPreferredCategory> UserPreferredCategories { get; set; } = new List<UserPreferredCategory>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
