using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Core.Constants.Authorization
{
    public static class PermissionScopeMap
    {
        private static readonly Dictionary<string, string> Map = new()
    {
        { Permissions.Product.Read,   Scopes.ProductRead },
        { Permissions.Product.Create, Scopes.ProductWrite },
        { Permissions.Product.Update, Scopes.ProductWrite },
        { Permissions.Product.Delete, Scopes.ProductWrite },
        { Permissions.Product.Hide,   Scopes.Admin },
    };

        public static string ToScope(string permission)
            => Map[permission];
    }

}
