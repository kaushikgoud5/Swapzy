using Swapzy.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities
{
    public class UserEntity : BaseAuditableEntity
    {
        public Guid Id {  get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public  string HashedPassword { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
