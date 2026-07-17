using Swapzy.Core.Common;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Core.Entities.Matches
{
    public class Match : BaseAuditableEntity
    {
        public int Id { get; set; }
        public Guid InterestedUserId { get; set; }  // User A (liked the product)
        public Guid SellerId { get; set; }           // User B (product owner)
        public int ProductId { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.Active;
        public DateTime? CancelledAt { get; set; }

        public UserEntity InterestedUser { get; set; } = null!;
        public UserEntity Seller { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }

    public enum MatchStatus
    {
        Active,
        Cancelled,
        Sold
    }
}
