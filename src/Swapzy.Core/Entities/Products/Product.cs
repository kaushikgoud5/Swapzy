using Swapzy.Core.Common;
using Swapzy.Core.Entities.Categories;
using Swapzy.Core.Entities.Users;
using Swapzy.Core.Enums;

namespace Swapzy.Core.Entities.Products
{
    public class Product : BaseAuditableEntity
    {
        public int Id { get; set; }
        public Guid OwnerId { get; set; }
        public int ProductCategoryId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Condition { get; set; }
        public double EstimatedValue { get; set; }
        public double OriginalValue { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Available;
        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int ViewCount { get; set; } = 0;
        public int WishlistCount { get; set; } = 0;
        public int SwapCount { get; set; } = 0;
        public int RejectCount { get; set; } = 0;
        public ProductMetadata? Metadata { get; set; }
        public Category Category { get; set; } = null!;
        public ProductLocation? Location { get; set; }
        public UserEntity Owner { get; set; } = null!;
    }
}
