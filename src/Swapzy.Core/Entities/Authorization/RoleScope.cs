using Swapzy.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Authorization
{
    public class RoleScope : BaseAuditableEntity
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Guid ScopeId { get; set; }
        public Scope Scope { get; set; }

    }
}
