using Swapzy.Core.Common;
using Swapzy.Core.Entities.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Users
{
    public class UserRole : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
