using Swapzy.Core.Common;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Entities.Users;
using Swapzy.Core.Enums;

namespace Swapzy.Core.Entities.Interests;

public class Interest : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public int ProductId { get; set; }
    public InterestStatus Status { get; set; } = InterestStatus.Pending;

    public UserEntity Buyer { get; set; } = null!;
    public UserEntity Seller { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
