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
        public UserEntity User { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

    }
}
