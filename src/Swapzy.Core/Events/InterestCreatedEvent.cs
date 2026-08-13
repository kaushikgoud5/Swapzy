namespace Swapzy.Core.Events;

public class InterestCreatedEvent : DomainEvent
{
    public Guid InterestId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public int ProductId { get; set; }
}
