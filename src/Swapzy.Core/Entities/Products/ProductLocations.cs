using Swapzy.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Products
{
    public class ProductLocation :BaseAuditableEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string Country { get; set; }
        public required string State { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Product Product { get; set; } = null!;
    }
}
