using Swapzy.Core.Common;
using Swapzy.Core.Entities.Categories;
namespace Swapzy.Core.Entities.Products
{
    public class ProductCategory : BaseAuditableEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
        public required Product Product { get; set; }
    }
}
