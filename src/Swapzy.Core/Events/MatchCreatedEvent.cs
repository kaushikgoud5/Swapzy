namespace Swapzy.Core.Events
{
    public class MatchCreatedEvent : DomainEvent
    {
        public int MatchId { get; set; }
        public Guid InterestedUserId { get; set; }
        public Guid SellerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
    }
}
