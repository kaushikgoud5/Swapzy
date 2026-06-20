using Swapzy.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T domainEvent, CancellationToken ct = default) where T : DomainEvent;
    }
}
