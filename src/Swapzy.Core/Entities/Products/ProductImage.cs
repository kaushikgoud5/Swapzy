using Swapzy.Core.Common;

namespace Swapzy.Core.Entities.Products
{
    public class ProductImage : BaseAuditableEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string S3Key { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public int DisplayOrder { get; set; }
        public Product Product { get; set; } = null!;
    }
}
