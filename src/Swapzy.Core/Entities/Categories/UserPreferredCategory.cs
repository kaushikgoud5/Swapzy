using Swapzy.Core.Common;
using Swapzy.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Entities.Categories
{
    public class UserPreferredCategory : BaseAuditableEntity
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public required UserEntity User { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }

    }
}
