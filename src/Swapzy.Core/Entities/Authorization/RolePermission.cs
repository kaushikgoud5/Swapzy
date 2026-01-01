using Swapzy.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Authorization
{
    public class RolePermission : BaseAuditableEntity
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
    
}
