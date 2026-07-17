using Swapzy.Core.Common;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Entities.Users;
using Swapzy.Core.Enums;

namespace Swapzy.Core.Entities.Swipes
{
    public class Swipe : BaseAuditableEntity
    {
        public long Id { get; set; }
        public Guid SwiperId { get; set; }
        public int ProductId { get; set; }
        public SwipeDirection Direction { get; set; }

        public UserEntity Swiper { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
