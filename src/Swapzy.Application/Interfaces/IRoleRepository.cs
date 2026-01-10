using Swapzy.Core.Entities.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetUserRolesAsync(Guid userId);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);


    }
}
