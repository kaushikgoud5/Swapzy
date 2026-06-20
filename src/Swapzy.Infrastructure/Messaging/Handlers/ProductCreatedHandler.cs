using Microsoft.Extensions.Logging;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Notifications;
using Swapzy.Core.Events;
using Swapzy.Infrastructure.Data;
using System.Text.Json;

namespace Swapzy.Infrastructure.Messaging.Handlers
{
    public class ProductCreatedHandler : IEventHandler
    {
        private readonly SwapzyDbContext _context;
        private readonly ILogger<ProductCreatedHandler> _logger;

        public string EventType => nameof(ProductCreatedEvent);

        public ProductCreatedHandler(SwapzyDbContext context, ILogger<ProductCreatedHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task HandleAsync(string messageBody, CancellationToken ct = default)
        {
            var evt = JsonSerializer.Deserialize<ProductCreatedEvent>(messageBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (evt == null) return;

            var notification = new Notification
            {
                UserId = evt.OwnerId,
                Title = "Product Listed!",
                Message = $"Your product \"{evt.Name}\" is now live.",
                EventType = evt.EventType,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "system"
            };

            await _context.Notifications.AddAsync(notification, ct);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Notification saved for user {UserId}, product {ProductId}", evt.OwnerId, evt.ProductId);
        }
    }
}
