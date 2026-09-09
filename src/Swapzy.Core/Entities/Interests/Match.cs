using Swapzy.Core.Common;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Core.Entities.Interests;

public class Match : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public Guid InterestId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public int ProductId { get; set; }

    public Interest Interest { get; set; } = null!;
    public UserEntity Buyer { get; set; } = null!;
    public UserEntity Seller { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
