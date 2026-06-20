namespace Swapzy.Application.Interfaces
{
    public interface IEventHandler
    {
        string EventType { get; }
        Task HandleAsync(string messageBody, CancellationToken ct = default);
    }
}
