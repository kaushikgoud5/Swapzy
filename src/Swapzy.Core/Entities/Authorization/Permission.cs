using Swapzy.Core.Common;
using Swapzy.Core.Enums;

namespace Swapzy.Core.Entities.Authorization
{
    public class Permission : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required ResourceType ResourceType { get; set; }
        public string Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<AuthorizationPolicy> AuthorizationPolicies { get; set; } = new List<AuthorizationPolicy>();


    }
}
