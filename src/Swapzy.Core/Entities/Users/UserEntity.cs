using Swapzy.Core.Common;
using Swapzy.Core.Entities.Categories;
using Swapzy.Core.Entities.Products;

namespace Swapzy.Core.Entities.Users
{
    public class UserEntity : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public string? PhoneNumber { get; set; }
        public ICollection<UserPreferredCategory> PreferredCategories { get; set; } = new List<UserPreferredCategory>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
