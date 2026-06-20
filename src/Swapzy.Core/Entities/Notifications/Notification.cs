using Swapzy.Core.Common;

namespace Swapzy.Core.Entities.Notifications
{
    public class Notification : BaseAuditableEntity
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
