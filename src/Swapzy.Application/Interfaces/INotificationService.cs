namespace Swapzy.Application.Interfaces
{
    public interface INotificationService
    {
        Task<object> GetAllAsync(Guid userId, bool unreadOnly, int page, int pageSize);
        Task MarkAsReadAsync(int notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
