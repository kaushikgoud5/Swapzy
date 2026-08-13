using Microsoft.Extensions.Logging;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Notifications;
using Swapzy.Core.Events;
using Swapzy.Infrastructure.Data;
using System.Text.Json;

namespace Swapzy.Infrastructure.Messaging.Handlers;

public class InterestCreatedHandler : IEventHandler
{
    private readonly SwapzyDbContext _context;
    private readonly ILogger<InterestCreatedHandler> _logger;

    public string EventType => nameof(InterestCreatedEvent);

    public InterestCreatedHandler(SwapzyDbContext context, ILogger<InterestCreatedHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(string messageBody, CancellationToken ct = default)
    {
        var evt = JsonSerializer.Deserialize<InterestCreatedEvent>(messageBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (evt == null) return;

        await _context.Notifications.AddAsync(new Notification
        {
            UserId = evt.SellerId,
            Title = "New Interest!",
            Message = $"A buyer has expressed interest in your product.",
            EventType = evt.EventType,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "system"
        }, ct);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Interest notification saved for seller {SellerId}, interest {InterestId}",
            evt.SellerId, evt.InterestId);
    }
}
