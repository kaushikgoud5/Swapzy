using Swapzy.Core.Common;

namespace Swapzy.Core.Entities.Users
{
    public class UserEntity : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
