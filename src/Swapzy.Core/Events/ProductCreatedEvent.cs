using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Events
{
    public class ProductCreatedEvent : DomainEvent
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; } 
        public Guid OwnerId { get; set; }
    }
}
