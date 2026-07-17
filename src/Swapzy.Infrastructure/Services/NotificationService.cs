using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Exceptions;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly SwapzyDbContext _context;

        public NotificationService(SwapzyDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetAllAsync(Guid userId, bool unreadOnly, int page, int pageSize)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId && n.DateDeleted == null);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.EventType,
                    n.IsRead,
                    n.CreatedOn
                })
                .ToListAsync();

            return new { notifications, page, pageSize, hasMore = notifications.Count == pageSize };
        }

        public async Task MarkAsReadAsync(int notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId)
                ?? throw new NotFoundException("Notification not found.");

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(n => n
                    .SetProperty(x => x.IsRead, true)
                    .SetProperty(x => x.ReadAt, DateTime.UtcNow));
        }
    }
}
