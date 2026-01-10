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
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;

        public string ResourceType { get; set; } = default!; 
        public RelationshipRule RelationshipRule { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
