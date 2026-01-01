using Swapzy.Core.Common;
using Swapzy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Authorization
{
    public class AuthorizationPolicy : BaseAuditableEntity
    {
        public long PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;

        public ResourceType ResourceType { get; set; } = default!; 
        public RelationshipRule RelationshipRule { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
