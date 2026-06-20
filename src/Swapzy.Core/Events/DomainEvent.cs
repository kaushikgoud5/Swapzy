using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Events
{
    public abstract class DomainEvent
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public string EventType => GetType().Name;
    }
}
