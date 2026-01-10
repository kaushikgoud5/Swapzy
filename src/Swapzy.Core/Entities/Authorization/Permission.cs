using Swapzy.Core.Common;
using Swapzy.Core.Enums;

namespace Swapzy.Core.Entities.Authorization
{
    public class Permission : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
