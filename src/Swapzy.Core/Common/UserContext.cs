using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Common
{
    public class UserContext
    {
        public Guid UserId { get; init; }
        public IReadOnlyList<string> Roles { get; init; } = [];
        public IReadOnlyList<string> Permissions { get; init; } = [];
    }

}
