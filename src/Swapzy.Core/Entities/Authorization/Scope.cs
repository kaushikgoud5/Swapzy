using Swapzy.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Authorization
{
    public class Scope : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public ICollection<RoleScope> RoleScopes { get; set; } = new List<RoleScope>();

    }
}
