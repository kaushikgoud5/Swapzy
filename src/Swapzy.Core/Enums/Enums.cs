using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Enums
{
    public enum RelationshipRule
    {
        OWNER,
        PARTICIPANT,
        ANY
    }
    public enum ResourceType
    {
        Prouduct,
        User,
        Category,
    }
    public enum ProductStatus
    {
        Available,
        Swapped,
        Unavailable
    }
}

