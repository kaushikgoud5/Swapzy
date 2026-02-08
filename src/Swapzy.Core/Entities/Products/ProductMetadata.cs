using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Products
{
    public class ProductMetadata
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string MetadataKey { get; set; } = string.Empty;
    }
}
