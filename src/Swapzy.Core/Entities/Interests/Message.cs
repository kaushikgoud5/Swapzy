using Swapzy.Core.Common;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Core.Entities.Interests;

public class Message : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; } = default!;

    public Match Match { get; set; } = null!;
    public UserEntity Sender { get; set; } = null!;
}
