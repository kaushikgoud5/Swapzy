using Swapzy.Core.Common;

namespace Swapzy.Core.Entities.Authorization
{
    public class Role : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
