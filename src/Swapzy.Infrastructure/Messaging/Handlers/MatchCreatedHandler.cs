using Microsoft.Extensions.Logging;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Notifications;
using Swapzy.Core.Events;
using Swapzy.Infrastructure.Data;
using System.Text.Json;

namespace Swapzy.Infrastructure.Messaging.Handlers
{
    public class MatchCreatedHandler : IEventHandler
    {
        private readonly SwapzyDbContext _context;
        private readonly ILogger<MatchCreatedHandler> _logger;

        public string EventType => nameof(MatchCreatedEvent);

        public MatchCreatedHandler(SwapzyDbContext context, ILogger<MatchCreatedHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task HandleAsync(string messageBody, CancellationToken ct = default)
        {
            var evt = JsonSerializer.Deserialize<MatchCreatedEvent>(messageBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (evt == null) return;

            // Notify the seller
            await _context.Notifications.AddAsync(new Notification
            {
                UserId = evt.SellerId,
                Title = "New Interest!",
                Message = $"Someone is interested in your product \"{evt.ProductName}\".",
                EventType = evt.EventType,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "system"
            }, ct);

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Match notification sent to seller {SellerId} for product {ProductId}",
                evt.SellerId, evt.ProductId);
        }
    }
}
